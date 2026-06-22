using Senswave.Abstractions.Messaging.Interfaces;
using Senswave.DataSources.Domain.Brokers.Brokers.Repositories;
using Senswave.DataSources.Domain.Brokers.Sessions.Entities;
using Senswave.DataSources.Domain.Brokers.Sessions.Enums;
using Senswave.DataSources.Domain.Brokers.Sessions.Repositories;
using Senswave.Integration.DataSource.BrokerConnection.Events;
using Senswave.Integration.DataTransfer.MessageReceivedFromDevice;
using System.Text.Json.Nodes;

namespace Senswave.DataSources.Infrastructure.Brokers.Features;

internal class MessageReceivedConsumer(IPublishMessageBus messageBus,
    ISubscribtionCommandRepository repository,
    ISessionCommandRepository sessionRepository,
    ILogger<MessageReceivedConsumer> logger) : IConsumer<MessageReceivedEvent>
{
    public async Task Consume(ConsumeContext<MessageReceivedEvent> context)
    {
        var subscribtion = await repository.GetSubscriptionByTopic(context.Message.BrokerId, context.Message.Topic, context.CancellationToken);

        if (subscribtion == null)
        {
            logger.LogWarning("[Broker: {brokerId}] [Topic: {topic}] Failed to match subscribtion for topic.",
                context.Message.BrokerId,
                context.Message.Topic);
            return;
        }

        await sessionRepository.CreateSessionLog(context.Message.SessionId, CreateLog(context.Message), context.CancellationToken);

        var message = new MessageReceivedFromDeviceEvent
        {
            BrokerId = context.Message.BrokerId,
            SubscribtionId = subscribtion.Id,
            Payload = context.Message.Payload
        };

        logger.LogInformation("[Broker: {brokerId}] [Subscribtion: {subscribtionId}] [Topic: {topic}] Publishing message to subscribers.",
            context.Message.BrokerId,
            subscribtion.Id,
            context.Message.Topic);

        await messageBus.Publish(message, context.CancellationToken);
    }

    #region Privates

    private static Log CreateLog(MessageReceivedEvent message)
    {
        var jsonObject = new JsonObject()
        {
            ["Topic"] = message.Topic
        };

        return new()
        {
            Type = SessionEventType.MessageReceived,

            Data = jsonObject.ToJsonString(),

            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
        };
    }

    #endregion
}
