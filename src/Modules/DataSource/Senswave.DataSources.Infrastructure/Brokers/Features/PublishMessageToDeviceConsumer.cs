using Senswave.DataSources.Domain.Brokers.Brokers.Repositories;
using Senswave.DataSources.Domain.Brokers.Clients.Proxy;
using Senswave.DataSources.Domain.Brokers.Sessions.Entities;
using Senswave.DataSources.Domain.Brokers.Sessions.Enums;
using Senswave.DataSources.Domain.Brokers.Sessions.Repositories;
using Senswave.Integration.DataTransfer.PublishMessageToDevice;
using Senswave.Integration.Shared;
using System.Text.Json.Nodes;

namespace Senswave.DataSources.Infrastructure.Brokers.Features;

public class PublishMessageToDeviceConsumer(
    ISubscribtionCommandRepository repository,
    ISessionCommandRepository sessionRepository,
    IClientProxy clientProxy,
    ILogger<PublishMessageToDeviceConsumer> logger)
    : IConsumer<PublishMessageToDeviceRequest>
{
    public async Task Consume(ConsumeContext<PublishMessageToDeviceRequest> context)
    {
        try
        {
            var subscribtion = await repository.GetSubscriptionWithBroker(context.Message.DataSourceReferenceId, context.CancellationToken);

            if (subscribtion is null)
            {
                logger.LogDebug("[Subscribtion: {subscriptionId}] Failed to find broker for subscription.", context.Message.DataSourceReferenceId);
                await context.RespondAsync(FailedToFindBroker);
                return;
            }

            var publishResponse = await clientProxy.PublishMessage(subscribtion.Broker.Id,
                context.Message.Payload,
                subscribtion.Topic,
                context.CancellationToken);

            var response = MessagePublished;

            if (publishResponse.IsFailure)
            {
                logger.LogError("[Subscribtion: {subscriptionId}] Failed to publish message to broker for subscription.", context.Message.DataSourceReferenceId);
                response = FailedToSendMessage;
            }
            else
            {
                logger.LogInformation("[Subscribtion: {subscriptionId}] Message published successfully to broker for subscription.", context.Message.DataSourceReferenceId);
            }

            var log = CreateLog(publishResponse.IsSuccess, subscribtion.Topic);
            await sessionRepository.CreateSessionLogForCurrentSession(subscribtion.Broker.Id, log, context.CancellationToken);


            await context.RespondAsync(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Subscribtion: {subscriptionId}] Failed to send message to device.", context.Message.DataSourceReferenceId);
            await context.RespondAsync(FailedToSendMessage);
        }
    }

    #region Privates
    private static Log CreateLog(bool isSuccess, string topic)
    {
        var jsonObject = new JsonObject()
        {
            ["Topic"] = topic
        };

        return new()
        {
            Type = isSuccess ? SessionEventType.MessagePublished : SessionEventType.FailedToPublishMessage,

            Data = jsonObject.ToJsonString(),

            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
        };
    }

    #endregion

    #region Messages
    private static PublishMessageToDeviceResponse FailedToFindBroker => new()
    {
        StatusCode = InternalRequestStatus.Failure,
        Error = Error.Failure("BrokerNotFound", "Failed to find broker by subscription id.")
    };

    private static PublishMessageToDeviceResponse FailedToSendMessage => new()
    {
        StatusCode = InternalRequestStatus.Failure,
        Error = Error.Failure("FailedToSendMessage", "Failed to send message to the device.")
    };

    private static PublishMessageToDeviceResponse MessagePublished => new()
    {
        StatusCode = InternalRequestStatus.Success,
    };

    #endregion
}
