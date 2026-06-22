using Senswave.Devices.Domain.Services;
using Senswave.Integration.Automations.TriggerOperationEvent;

namespace Senswave.Devices.Infrastructure.Consumers;

public class ExternalDeviceActionConsumer(
    ILogger<ExternalDeviceActionConsumer> logger,
    IActionService actionService)
    : IConsumer<ExternalDeviceActionRequest>
{
    public async Task Consume(ConsumeContext<ExternalDeviceActionRequest> context)
    {
        var request = context.Message;

        foreach (var operationWithValue in request.OperationsWithValues)
        {
            logger.LogInformation("[Operation: {operation}] Assembling opration with automation.",
                operationWithValue.OperationId);

            var result = await actionService.ExternalAction(
                operationWithValue.OperationId,
                operationWithValue.Value,
                context.CancellationToken);

            if (result.IsFailure)
            {
                // TODO: 189 Automation History
                logger.LogError("[Operation: {operation}] Failed to assemble opration with automation. Error code : {error}. Error description : {errorDesc}",
                    operationWithValue.OperationId,
                    result.Errors.FirstOrDefault()?.Code,
                    result.Errors.FirstOrDefault()?.Description);
            }
        }
    }

}