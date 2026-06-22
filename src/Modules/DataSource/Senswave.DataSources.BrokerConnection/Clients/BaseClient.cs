using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using Senswave.Abstractions.Collections;
using Senswave.Abstractions.Messaging.Interfaces;
using Senswave.Abstractions.Resulting;
using Senswave.DataSources.BrokerConnection.RateLimiters;
using Senswave.DataSources.Domain.Brokers.Clients.Entities;
using Senswave.DataSources.Domain.Brokers.Clients.Options;
using Senswave.DataSources.Domain.Diagnostics;
using Senswave.Integration.DataSource.BrokerConnection.Events;
using Senswave.Integration.DataSource.BrokerConnection.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace Senswave.DataSources.BrokerConnection.Clients;

public abstract class BaseClient : IClient
{
    protected static CancellationToken DefaultCanncellation => new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;

    private const int MaxTimeoutMiliseconds = 1000;

    #region Errors
    protected static Error AlreadyConnected => Error.Failure("AlreadyConnected", "Client is already connected.");
    protected static Error AlreadyDisconnected => Error.Failure("AlreadyDisconnected", "Client is already disconnected.");
    protected static Error ClientNotConnected => Error.Failure("ClientNotConnected", "Client is not connected.");
    protected static Error FailedToAddSubscribtion => Error.Failure("FailedToAddSubscribtion", "Failed to add subscription to the broker client.");
    protected static Error FailedToManageClientState => Error.Failure("FailedToManageClientState", "Client state is managed by someone else, please try again.");
    protected static Error OperationCancelled => Error.Failure("OperationCancelled", "The operation was cancelled.");
    protected static Error FailedToReconnect => Error.Failure("FailedToReconnect", "Failed to reconnect to the broker.");
    protected static Error FailedToStartConnectionWithBroker => Error.Failure("FailedToStartConnectionWithBroker", "Failed to start connection with the broker.");

    #endregion

    #region Services

    protected readonly ConcurrentHashSet<string> _subscribtions;

    protected readonly ConcurrentBag<(string, DateTime)> _receivedMessages = [];

    protected readonly MqttClientOptionsBuilder _optionsBuilder;

    protected readonly ClientOptions _options;

    protected readonly ILogger<BaseClient> _logger;

    protected readonly IMessageBus _bus;

    protected readonly IMqttClient _client;

    protected readonly IBrokerRateLimiter _rateLimiter;

    protected readonly IDataSourcesActivityProvider _activityProvider;

    #endregion

    #region Properties

    private bool clientStoppedByUser = false;

    public bool AllowReconnecting { get; private set; } = true;

    public bool IsConnected => _client.IsConnected;

    private bool _remove;
    public bool Remove
    {
        get => _remove;
        private set
        {
            DisconnectedAtUtc = value ? DateTime.UtcNow : DateTime.MaxValue;
            _remove = value;
        }
    }

    public DateTime DisconnectedAtUtc { get; private set; } = DateTime.MaxValue;

    public Guid BrokerId { get; private set; }

    public Guid SessionId { get; private set; }

    #endregion

    #region Semaphores

    protected readonly SemaphoreSlim _stateSemaphore = new(1, 1);

    protected readonly SemaphoreSlim _recivedMessagesSemaphore = new(1, 1);

    #endregion

    protected BaseClient(
        Guid brokerId,
        Guid sessionId,
        MqttClientOptionsBuilder optionsBuilder,
        ClientOptions options,
        IBrokerRateLimiter brokerRateLimiter,
        IList<string> subscribtions,
        IMqttClient client,
        IMessageBus messageBus,
        IDataSourcesActivityProvider activityProvider,
        ILogger<BaseClient> logger)
    {

        BrokerId = brokerId;
        SessionId = sessionId;

        _rateLimiter = brokerRateLimiter;
        _optionsBuilder = optionsBuilder;
        _options = options;
        _subscribtions = [.. subscribtions];
        _bus = messageBus;
        _logger = logger;
        _activityProvider = activityProvider;

        _client = client;
        _client.ConnectedAsync += OnConnected;
        _client.DisconnectedAsync += OnDisconnected;
        _client.ApplicationMessageReceivedAsync += OnReceived;

        Remove = false;
    }

    public async virtual Task<Result> Restart(CancellationToken cancellationToken = default)
    {
        var enteredSemaphore = false;
        try
        {
            using var activity = _activityProvider.StartActivity("Mqtt /restart");
            activity?.AddTag("mqtt.client.broker_id", BrokerId);
            activity?.AddTag("mqtt.client.session_id", SessionId);

            enteredSemaphore = await _stateSemaphore.WaitAsync(MaxTimeoutMiliseconds, cancellationToken);

            if (!enteredSemaphore)
            {
                _logger.LogWarning("[BrokerId: {brokerId}] [SessionId: {sessionId}] Cannot restart client, client state is being changed.",
                    BrokerId, SessionId);

                return Result.Failure(FailedToManageClientState);
            }

            activity?.AddEvent(new("Semaphore acquired."));

            if (_client.IsConnected)
            {
                _logger.LogInformation("[BrokerId: {brokerId}] [SessionId: {sessionId}] Cannot restart client when client is already connected.",
                    BrokerId, SessionId);

                return Result.Failure(AlreadyConnected);
            }

            if (clientStoppedByUser)
            {
                _logger.LogWarning("[BrokerId: {brokerId}] [SessionId: {sessionId}] Cannot restart client, client was stopped manually.",
                    BrokerId, SessionId);

                return Result.Failure(AlreadyDisconnected);
            }

            await _client.ReconnectAsync(cancellationToken);

            _logger.LogInformation("[BrokerId: {brokerId}] [SessionId: {sessionId}] Reconnecting client manually.",
                BrokerId, SessionId);

            return Result.Success();
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("[BrokerId: {brokerId}] [SessionId: {sessionId}] Restart operation was cancelled.",
                BrokerId, SessionId);
            return Result.Failure(OperationCancelled);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[BrokerId: {brokerId}] [SessionId: {sessionId}] Failed to manually reconnect.",
                BrokerId, SessionId);
            return Result.Failure(FailedToReconnect);
        }
        finally
        {
            if (enteredSemaphore)
                _stateSemaphore.Release();
        }
    }

    /// <summary>
    /// Function starts mqtt client.
    /// Funtion should not update Connection throught message bus.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async virtual Task<Result> Start(string username, string password, CancellationToken cancellationToken = default)
    {
        bool enteredSemaphore = false;

        try
        {
            using var activity = _activityProvider.StartActivity("Mqtt /connect");
            activity?.AddTag("mqtt.client.broker_id", BrokerId);
            activity?.AddTag("mqtt.client.session_id", SessionId);

            enteredSemaphore = await _stateSemaphore.WaitAsync(MaxTimeoutMiliseconds, cancellationToken);

            if (!enteredSemaphore)
            {
                _logger.LogWarning("[BrokerId: {brokerId}] [SessionId: {sessionId}] Cannot start client, client state is being changed by different operation.",
                    BrokerId,
                    SessionId);

                return Result.Failure(FailedToManageClientState);
            }

            activity?.AddEvent(new("Semaphore acquired."));

            if (_client.IsConnected)
            {
                _logger.LogWarning("[BrokerId: {brokerId}] [SessionId: {sessionId}] Cannot start client, client state is being changed.",
                    BrokerId,
                    SessionId);

                return Result.Failure(AlreadyConnected);
            }

            if (clientStoppedByUser)
            {
                _logger.LogWarning("[BrokerId: {brokerId}] [SessionId: {sessionId}] Cannot start client, client was stopped manually.",
                    BrokerId,
                    SessionId);
                return Result.Failure(AlreadyDisconnected);
            }

            _optionsBuilder
                .WithCredentials(username, password);

            var options = _optionsBuilder.Build();

            _logger.LogInformation("[BrokerId: {brokerId}] [SessionId: {sessionId}] Starting connecting client.",
                BrokerId, SessionId);

            var result = await _client.ConnectAsync(options, cancellationToken);

            return Result.Success();
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("[BrokerId: {brokerId}] [SessionId: {sessionId}] Start operation was cancelled.",
                BrokerId, SessionId);
            return Result.Failure(OperationCancelled);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[BrokerId: {brokerId}] [SessionId: {sessionId}] Failed to start connection with broker.",
                BrokerId, SessionId);
            return Result.Failure(FailedToStartConnectionWithBroker);
        }
        finally
        {
            if (enteredSemaphore)
                _stateSemaphore.Release();
        }
    }

    /// <summary>
    /// Function stops mqtt client.
    /// Funtion should not update Connection throught message bus.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async virtual Task<Result> Stop(CancellationToken cancellationToken = default)
    {
        bool enteredSemaphore = false;
        try
        {
            using var activity = _activityProvider.StartActivity("Mqtt /disconnect");
            activity?.AddTag("mqtt.client.broker_id", BrokerId);
            activity?.AddTag("mqtt.client.session_id", SessionId);

            enteredSemaphore = await _stateSemaphore.WaitAsync(MaxTimeoutMiliseconds, cancellationToken);

            if (!enteredSemaphore)
            {
                _logger.LogWarning("[BrokerId: {brokerId}] [SessionId: {sessionId}] Cannot start client, client state is being changed by different operation.",
                    BrokerId,
                    SessionId);

                return Result.Failure(FailedToManageClientState);
            }

            activity?.AddEvent(new("Semaphore acquired."));

            if (!_client.IsConnected)
            {
                _logger.LogInformation("[BrokerId: {brokerId}] [SessionId: {sessionId}] Cannot stop client when client is already disconnected.",
                    BrokerId,
                    SessionId);

                return Result.Failure(AlreadyDisconnected);
            }

            _logger.LogInformation("[BrokerId: {brokerId}] [SessionId: {sessionId}] Stopping client.",
                BrokerId,
                SessionId);

            Remove = false;
            _client.DisconnectedAsync -= OnDisconnected;
            _client.ConnectedAsync -= OnConnected;
            _client.ApplicationMessageReceivedAsync -= OnReceived;
            AllowReconnecting = false;
            clientStoppedByUser = true;

            await _client.DisconnectAsync(cancellationToken: cancellationToken);

            await _bus.Publish(Disconnected("User disconnect."), cancellationToken);

            _logger.LogInformation("[BrokerId: {brokerId}] [SessionId: {sessionId}] Client stopped.",
                BrokerId,
                SessionId);

            return Result.Success();
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("[BrokerId: {brokerId}] [SessionId: {sessionId}] Stop operation was cancelled.",
                BrokerId,
                SessionId);

            return Result.Failure(OperationCancelled);
        }
        finally
        {
            if (enteredSemaphore)
                _stateSemaphore.Release();
        }
    }

    public async virtual Task<Result> Publish(string topic, string payload, CancellationToken cancellation = default)
    {
        try
        {
            using var activity = _activityProvider.StartActivity("Mqtt /publish");
            activity?.AddTag("mqtt.client.broker_id", BrokerId);
            activity?.AddTag("mqtt.client.session_id", SessionId);

            if (!_client.IsConnected)
            {
                _logger.LogInformation("[BrokerId: {brokerId}] [SessionId: {sessionId}] Cannot publish message when client is disconnected.",
                    BrokerId, SessionId);

                return Result.Failure(ClientNotConnected);
            }

            await _client.PublishAsync(BuildMessage(topic, payload), cancellation);

            _logger.LogInformation("[BrokerId: {brokerId}] [SessionId: {sessionId}] [Topic: {topic}] Message published.",
                BrokerId, SessionId, topic);

            return Result.Success();
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("[BrokerId: {brokerId}] [SessionId: {sessionId}] Publish operation was cancelled.",
                BrokerId, SessionId);
            return Result.Failure(OperationCancelled);
        }
    }

    public async virtual Task<Result> Subscribe(string topic, CancellationToken cancellationToken = default)
    {
        try
        {
            using var activity = _activityProvider.StartActivity("Mqtt /subscribe");

            activity?.AddTag("mqtt.client.broker_id", BrokerId);
            activity?.AddTag("mqtt.client.session_id", SessionId);

            if (_subscribtions.Contains(topic))
            {
                _logger.LogInformation("[BrokerId: {brokerId}] [SessionId: {sessionId}] [Topic: {topic}] Subscribtion already exists.",
                    BrokerId, SessionId, topic);
                return Result.Success();
            }

            _subscribtions.Add(topic);

            _logger.LogInformation("[BrokerId: {brokerId}] [SessionId: {sessionId}] [Topic: {topic}] Subscribtion added.",
                BrokerId, SessionId, topic);

            if (!_client.IsConnected)
                return Result.Success();

            await _client.SubscribeAsync(BuildSubscribtion(topic), cancellationToken);

            _logger.LogInformation("[BrokerId: {brokerId}] [SessionId: {sessionId}] [Topic: {topic}] Subscribtion added to working client.",
                BrokerId, SessionId, topic);

            return Result.Success();
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("[BrokerId: {brokerId}] [SessionId: {sessionId}] Subscribe operation was cancelled.",
                BrokerId, SessionId);

            return Result.Failure(OperationCancelled);
        }
    }

    public async virtual Task<Result> Unsubscribe(string topic, CancellationToken cancellationToken = default)
    {
        try
        {
            using var activity = _activityProvider.StartActivity("Mqtt /unsubscribe");
            activity?.AddTag("mqtt.client.broker_id", BrokerId);
            activity?.AddTag("mqtt.client.session_id", SessionId);

            var removed = _subscribtions.Remove(topic);

            if (!removed)
            {
                _logger.LogWarning("[BrokerId: {brokerId}] [SessionId: {sessionId}] [Topic: {topic}] Failed to remove subscribion.", BrokerId, SessionId, topic);
                return Result.Failure([]);
            }

            if (_client.IsConnected)
            {
                await _client.UnsubscribeAsync(BuildUnsubscribtion(topic), cancellationToken);
            }

            _logger.LogInformation("[BrokerId: {brokerId}] [SessionId: {sessionId}] [Topic: {topic}] Subscribtion removed.",
                BrokerId, SessionId, topic);

            return Result.Success();
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("[BrokerId: {brokerId}] [SessionId: {sessionId}] Unsubscribe operation was cancelled.", BrokerId, SessionId);
            return Result.Failure([OperationCancelled]);
        }
    }

    #region Callbacks

    protected virtual async Task OnReceived(MqttApplicationMessageReceivedEventArgs args)
    {
        Activity.Current = null;
        using var activity = _activityProvider.StartActivity("Mqtt /callback/received", ActivityKind.Consumer);

        activity?.AddTag("mqtt.client.broker_id", BrokerId);
        activity?.AddTag("mqtt.client.session_id", SessionId);
        activity?.AddTag("mqtt.event.topic", args.ApplicationMessage.Topic);

        var message = Encoding.UTF8.GetString(args.ApplicationMessage.PayloadSegment);

        await ProcessReceivedMessage(args.ApplicationMessage.Topic, message);
    }

    /// <summary>
    /// Function invoked when broker is connecting.
    /// Function also aims to update Connection status if invoked.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    protected virtual async Task OnConnected(MqttClientConnectedEventArgs args)
    {
        Activity.Current = null;
        using var activity = _activityProvider.StartActivity("Mqtt /callback/connected", ActivityKind.Consumer);
        activity?.AddTag("mqtt.client.broker_id", BrokerId);
        activity?.AddTag("mqtt.client.session_id", SessionId);

        try
        {
            _logger.LogInformation("[BrokerId: {brokerId}] [SessionId: {sessionId}] Client connected.",
                BrokerId, SessionId);

            foreach (var topic in _subscribtions)
            {
                await _client.SubscribeAsync(BuildSubscribtion(topic));
                _logger.LogDebug("[BrokerId: {brokerId}] [SessionId: {sessionId}] Subscribed to topic: {topic}",
                    BrokerId, SessionId, topic);
            }

            activity?.AddEvent(new("Subscribtions recreated"));

            await _bus.Publish(Connected(), DefaultCanncellation);

            Remove = false;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("[BrokerId: {brokerId}] [SessionId: {sessionId}] Cancellation in OnConnected callback.",
                BrokerId, SessionId);
        }
    }

    /// <summary>
    /// Function invoked when broker is disconnecting.
    /// Function also aims to update Connection status if invoked.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    protected virtual async Task OnDisconnected(MqttClientDisconnectedEventArgs arg)
    {
        try
        {
            if (Remove)
                return;

            Activity.Current = null;
            using var activity = _activityProvider.StartActivity("Mqtt /callback/disconnected", ActivityKind.Consumer);
            activity?.AddTag("mqtt.disconnect.reason", arg.Reason);
            activity?.AddTag("mqtt.client.broker_id", BrokerId);
            activity?.AddTag("mqtt.client.session_id", SessionId);

            if (arg.Reason == MqttClientDisconnectReason.SessionTakenOver)
            {
                _logger.LogError("[BrokerId: {brokerId}] [SessionId: {sessionId}] Session was taken over, client stopping.",
                    BrokerId, SessionId);
                await _bus.Publish(Disconnected(arg.Reason.ToString()), DefaultCanncellation);
                activity?.AddEvent(new("Session taken over."));
                Remove = true;
                return;
            }

            if (arg.Reason == MqttClientDisconnectReason.NotAuthorized)
            {
                _logger.LogError("[BrokerId: {brokerId}] [SessionId: {sessionId}] Could not authorize to client",
                    BrokerId, SessionId);
                await _bus.Publish(Disconnected(arg.Reason.ToString()), DefaultCanncellation);
                activity?.AddEvent(new("Not authorized."));
                Remove = true;
                return;
            }


            var reason = arg.Reason.ToString();

            if (clientStoppedByUser)
            {
                _logger.LogInformation("[BrokerId: {brokerId}] [SessionId: {sessionId}] Client disconnected manually.",
                    BrokerId, SessionId);
            }
            if (AllowReconnecting)
            {
                _logger.LogWarning("[BrokerId: {brokerId}] [SessionId: {sessionId}] Client disconnected with reason: {reason}. Reconnecting...",
                    BrokerId, SessionId, reason);
                await Reconnect(reason);
            }
            else
            {
                _logger.LogInformation("[BrokerId: {brokerId}] [SessionId: {sessionId}] Client not connected, but reconnecting is not allowed.",
                    BrokerId, SessionId);
                await _bus.Publish(Disconnected(reason), DefaultCanncellation);
                activity?.AddEvent(new("Reconnection not allowed."));
                Remove = true;
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("[BrokerId: {brokerId}] [SessionId: {sessionId}] Cancellation in OnDisconnected callback.",
                BrokerId, SessionId);
        }
    }

    #endregion

    #region Protected

    protected virtual async Task ProcessReceivedMessage(string topic, string message)
    {
        using var activity = _activityProvider.StartActivity("Mqtt /callback/received");
        activity?.AddTag("mqtt.event.length", message.Length);

        try
        {
            if (!_subscribtions.Contains(topic))
            {
                _logger.LogWarning("[BrokerId: {brokerId}] [SessionId: {sessionId}] Unsubscribbing to topic that should not be subscribed.",
                    BrokerId,
                    SessionId);
                activity?.AddEvent(new("Topis should be ignored."));
                await _client.UnsubscribeAsync(topic);
                return;
            }

            var canProcess = await _rateLimiter.CanProcessMessage(BrokerId, SessionId, topic);

            if (!canProcess)
            {
                _logger.LogInformation("[BrokerId: {brokerId}] [SessionId: {sessionId}] [Topic: {topic}] Message could not be processed. Received message too fast",
                    BrokerId,
                    SessionId,
                    topic);

                activity?.AddEvent(new("Message rate-limited."));
                await _bus.Publish(MessageIgnored(topic));
                return;
            }

            var receivedMessage = new MessageReceivedEvent()
            {
                Payload = message,
                Topic = topic,
                BrokerId = BrokerId,
                SessionId = SessionId,
            };

            _logger.LogTrace("[BrokerId: {brokerId}] [SessionId: {sessionId}] [Topic: {topic}] Received message.",
                BrokerId,
                SessionId,
                topic);

            await _bus.Publish(receivedMessage, DefaultCanncellation);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("[BrokerId: {brokerId}] [SessionId: {sessionId}] Cancellation in Receive callback.",
                BrokerId,
                SessionId);

            activity?.AddEvent(new("Processing timeout."));
        }
    }


    protected async Task Reconnect(string reason)
    {
        var enteredSemaphore = false;
        try
        {
            using var activity = _activityProvider.StartActivity("Mqtt /callback/reconnect", ActivityKind.Consumer);

            enteredSemaphore = await _stateSemaphore.WaitAsync(100);

            // If no access return
            if (!enteredSemaphore)
            {
                _logger.LogWarning("[BrokerId: {brokerId}] [SessionId: {sessionId}] Cannot reconnect, connection is already restarting.", BrokerId, SessionId);
                activity?.AddEvent(new("No semaphore access."));
                return;
            }

            //If reconnecting failed return
            if (Remove)
                return;

            await _bus.Publish(ReconnectingLog(reason), DefaultCanncellation);

            activity?.AddTag("mqtt.configuration.max_reconnects:", _options.ReconnectAttempts);

            for (int tries = 1; tries < _options.ReconnectAttempts + 1; tries++)
            {
                try
                {
                    activity?.AddEvent(new($"Reconnect: {tries}"));
                    await _client.ReconnectAsync(DefaultCanncellation);
                }
                catch
                {
                    _logger.LogError("[BrokerId: {brokerId}] [SessionId: {sessionId}] [ {try}/{total} ] Failed to reconnect.", BrokerId, SessionId, tries, _options.ReconnectAttempts);
                }

                if (_client.IsConnected)
                {
                    activity?.AddTag("mqtt.client.reconnects:", tries);
                    activity?.AddEvent(new($"Reconnected"));
                    return;
                }

                var delay = _options.ReconnectMilisecondSpanBetweenAttempts * tries;
                await Task.Delay(delay);

                if (_client.IsConnected)
                {
                    activity?.AddTag("mqtt.client.reconnects:", tries);
                    activity?.AddEvent(new($"Reconnected"));
                    return;
                }
            }

            activity?.AddEvent(new($"Failed to reconnect"));

            await _bus.Publish(Disconnected(reason), DefaultCanncellation);
            Remove = true;
        }
        finally
        {
            if (enteredSemaphore)
                _stateSemaphore.Release();
        }
    }

    protected virtual MqttClientSubscribeOptions BuildSubscribtion(string topic)
    {
        var topicFilter = new MqttTopicFilterBuilder()
            .WithTopic(topic)
            .Build();

        return new MqttClientSubscribeOptionsBuilder()
            .WithTopicFilter(topicFilter)
            .Build();
    }

    protected static MqttClientUnsubscribeOptions BuildUnsubscribtion(string topic)
    {
        var topicFilter = new MqttTopicFilterBuilder()
            .WithTopic(topic)
            .Build();

        return new MqttClientUnsubscribeOptionsBuilder()
            .WithTopicFilter(topicFilter)
            .Build();
    }

    protected static MqttApplicationMessage BuildMessage(string topic, string payload) => new MqttApplicationMessageBuilder()
        .WithTopic(topic)
        .WithPayload(payload)
        .Build();

    #endregion

    #region Messages

    protected ConnectedLog Connected() => new()
    {
        BrokerId = BrokerId,
        SessionId = SessionId,
    };

    protected MessageIgnoredLog MessageIgnored(string topic) => new()
    {
        BrokerId = BrokerId,
        SessionId = SessionId,
        Topic = topic
    };

    protected DisconnectedLog Disconnected(string reason) => new()
    {
        BrokerId = BrokerId,
        SessionId = SessionId,
        Reason = reason
    };

    protected ReconnectingLog ReconnectingLog(string reason) => new()
    {
        BrokerId = BrokerId,
        SessionId = SessionId,
        Reason = reason
    };

    #endregion

    #region IDisposable

    public void Dispose()
    {
        _subscribtions.Clear();
        _stateSemaphore.Dispose();
        _client.Dispose();
        GC.SuppressFinalize(this);
    }

    #endregion
}
