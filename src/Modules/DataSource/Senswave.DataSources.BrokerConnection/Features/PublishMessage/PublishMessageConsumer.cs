using MassTransit;
using Microsoft.Extensions.Logging;
using Senswave.Abstractions.Resulting;
using Senswave.DataSources.BrokerConnection.Interfaces;
using Senswave.Integration.DataSource.BrokerConnection.PublishMessage;
using Senswave.Integration.Shared;

namespace Senswave.DataSources.BrokerConnection.Features.PublishMessage;

public class PublishMessageConsumer(
    IClientService clientService,
    ILogger<PublishMessageConsumer> logger) : IConsumer<PublishMessageRequest>
{
    public async Task Consume(ConsumeContext<PublishMessageRequest> context)
    {
        var result = await clientService.PublishToClient(context.Message.BrokerId, context.Message.Topic, context.Message.Payload, context.CancellationToken);

        var status = result.IsSuccess ? Success() : Failure(result.Errors);

        if (result.IsSuccess)
        {
            logger.LogInformation("[Broker: {brokerId}] Message published to topic: {topic}.", context.Message.BrokerId, context.Message.Topic);
        }
        else
        {
            logger.LogError("[Broker: {brokerId}] Failed to publish message to topic: {topic}.", context.Message.BrokerId, context.Message.Topic);
        }

        await context.RespondAsync(status);
    }

    #region Messages

    public static PublishMessageResponse Failure(Error[] errors) => new()
    {
        StatusCode = InternalRequestStatus.Failure,
        Error = errors.Length > 0 ? (Error)errors[0] : Error.None
    };


    public static PublishMessageResponse Success() => new()
    {
        StatusCode = InternalRequestStatus.Success
    };

    #endregion
}
