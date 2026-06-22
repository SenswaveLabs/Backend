using Senswave.Devices.Domain.Operations.Services;
using Senswave.Devices.Domain.Services;
using Senswave.Integration.DataTransfer.MessageReceivedFromDevice;

namespace Senswave.Devices.Infrastructure.Consumers;

public class MessageForProcessingConsumer(
    IOperationActionService operationService,
    IActionService actionService,
    ILogger<MessageForProcessingConsumer> logger) : IConsumer<MessageReceivedFromDeviceEvent>
{
    public async Task Consume(ConsumeContext<MessageReceivedFromDeviceEvent> context)
    {
        logger.LogDebug("[Subscribtion: {subscribtionId}] Processing message from data source module.",
            context.Message.SubscribtionId);

        var result = await operationService.IncomingOperationActionProcessing(
            context.Message.SubscribtionId,
            context.Message.Payload,
            context.CancellationToken);

        if (result.IsFailure)
        {
            logger.LogError("[Subscribtion: {subscribtionId}] Processing message from data source module failed.",
                context.Message.SubscribtionId);
            return;
        }

        logger.LogInformation("[Subscribtion: {subscribtionId}] Message from data source module processed successfully. Operation IDs: {operationIds}",
            context.Message.SubscribtionId, string.Join(", ", result.Data));

        await actionService.IncomingActionProcessing(result.Data, context.CancellationToken);
    }
}
