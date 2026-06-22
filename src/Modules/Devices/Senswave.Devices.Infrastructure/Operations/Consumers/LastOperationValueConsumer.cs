using Senswave.Devices.Domain.Operations.Repositories;
using Senswave.Integration.Devices.LastOperationValue;
using Senswave.Integration.Shared;

namespace Senswave.Devices.Infrastructure.Operations.Consumers;

public class LastOperationValueConsumer(
    IOperationQueryRepository queryRepository,
    ILogger<LastOperationValueConsumer> logger) : IConsumer<LastOperationValueRequest>
{
    public async Task Consume(ConsumeContext<LastOperationValueRequest> context)
    {
        var operation = await queryRepository.GetOperation(context.Message.OperationId, context.CancellationToken);

        if (operation is null || operation.Values.Count == 0)
        {
            logger.LogWarning(
                "[OperationId: {OperationId}] Last operation value request failed. Operation not found or has no values.",
                context.Message.OperationId);
            await context.RespondAsync(ErrorResponse());
            return;
        }

        var response = new LastOperationValueResponse
        {
            LastValue = operation!.Values.Last().Value
        };

        logger.LogInformation(
            "[OperationId: {OperationId}] Last operation value request processed successfully. Last value: {LastValue}",
            context.Message.OperationId, response.LastValue);
        await context.RespondAsync(response);
    }

    private LastOperationValueResponse ErrorResponse() => new()
    {
        StatusCode = InternalRequestStatus.Failure
    };
}