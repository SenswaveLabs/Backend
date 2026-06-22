using Microsoft.Extensions.Logging;
using MQTTnet.Client;
using Senswave.Abstractions.Messaging.Interfaces;
using Senswave.Abstractions.Resulting;
using Senswave.DataSources.BrokerConnection.RateLimiters;
using Senswave.DataSources.Domain.Brokers.Clients.Options;
using Senswave.DataSources.Domain.Diagnostics;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace Senswave.DataSources.BrokerConnection.Clients.MqttV3;

public class MqttV3Client(
        Guid brokerId,
        Guid sessionId,
        MqttClientOptionsBuilder optionsBuilder,
        ClientOptions options,
        IBrokerRateLimiter rateLimiter,
        IList<string> subscribtions,
        IMqttClient client,
        IMessageBus messageBus,
        IDataSourcesActivityProvider activityProvider,
        ILogger<BaseClient> logger)
    : BaseClient(brokerId, sessionId, optionsBuilder, options, rateLimiter, subscribtions, client, messageBus, activityProvider, logger)
{

    #region Errors
    protected static Error FailedToSendMessage => Error.Failure("FailedToSendMessage", "Failed to send message to the device.");

    #endregion

    #region Properties

    protected readonly ConcurrentDictionary<string, ConcurrentDictionary<DateTime, string>> _publishedMessages = new();

    #endregion

    #region Semaphores

    protected readonly SemaphoreSlim _publishSemaphore = new(1, 1);

    #endregion

    public override async Task<Result> Publish(string topic, string payload, CancellationToken cancellation = default)
    {
        var enteredSemaphore = false;

        try
        {
            if (!_client.IsConnected)
            {
                _logger.LogDebug("[BrokerId: {brokerId}] [SessionId: {sessionId}] Cannot publish message when client is disconnected.", BrokerId, SessionId);
                return Result.Failure([ClientNotConnected]);
            }

            var semaphorePublishTimeout = _options.MqttV3.PublishTimeoutMiliseconds;

            enteredSemaphore = await _stateSemaphore.WaitAsync(semaphorePublishTimeout, cancellation);

            // If no access return
            if (!enteredSemaphore)
                return Result.Failure([FailedToSendMessage]);

            if (!_publishedMessages.TryGetValue(topic, out var value))
            {
                var dict = new ConcurrentDictionary<DateTime, string>();
                dict[DateTime.UtcNow] = payload;
                _publishedMessages.TryAdd(topic, dict);
            }
            else
                value[DateTime.UtcNow] = payload;

            await _client.PublishAsync(BuildMessage(topic, payload), cancellation);

            _logger.LogInformation("[BrokerId: {brokerId}] [SessionId: {sessionId}] [Topic: {topic}] Message published.", BrokerId, SessionId, topic);

            return Result.Success();
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("[BrokerId: {brokerId}] [SessionId: {sessionId}] Publish operation was cancelled.", BrokerId, SessionId);
            return Result.Failure([OperationCancelled]);
        }
        finally
        {
            if (enteredSemaphore)
                _stateSemaphore.Release();
        }
    }

    #region Callbacks

    protected override async Task OnReceived(MqttApplicationMessageReceivedEventArgs args)
    {
        Activity.Current = null;

        using var activity = _activityProvider.StartActivity("Mqtt /callback/received", ActivityKind.Consumer);
        activity?.AddTag("mqtt.client.broker_id", BrokerId);
        activity?.AddTag("mqtt.client.session_id", SessionId);
        activity?.AddTag("mqtt.event.topic", args.ApplicationMessage.Topic);

        var payloadLength = args.ApplicationMessage.PayloadSegment.Count;

        if (payloadLength > _options.MaximalSizeOfMessageInBytes)
        {
            _logger.LogWarning("[BrokerId: {brokerId}] [SessionId: {sessionId}] [Topic: {topic}] Message is too long ignoring message.",
                BrokerId,
                SessionId,
                args.ApplicationMessage.Topic);

            activity?.AddEvent(new("Message is too long."));
            return;
        }

        activity?.AddTag("mqtt.event.length", payloadLength);

        var message = Encoding.UTF8.GetString(args.ApplicationMessage.PayloadSegment);

        if (_publishedMessages.TryGetValue(args.ApplicationMessage.Topic, out var dictionary))
        {
            var messaeTimeout = TimeSpan.FromMilliseconds(_options.MqttV3.MessageTimeoutMiliseconds);

            var now = DateTime.UtcNow;

            dictionary!.Where(x => x.Key + messaeTimeout < now)
                .Select(x => x.Key)
                .ToList()
                .ForEach(x => dictionary!.TryRemove(x, out _));

            if (dictionary!.Any(x => x.Value == message))
            {
                dictionary!.TryRemove(dictionary.First(x => x.Value == message).Key, out _);
                _logger.LogDebug("[BrokerId: {brokerId}] [SessionId: {sessionId}] [Topic: {topic}] Ignoring duplicate message.",
                BrokerId,
                SessionId,
                args.ApplicationMessage.Topic);

                activity?.AddEvent(new("Ignoring duplicate."));

                return;
            }
        }

        await ProcessReceivedMessage(args.ApplicationMessage.Topic, message);
    }

    #endregion
}
