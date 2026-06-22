using Senswave.Devices.Domain.Operations.Repositories;
using Senswave.Integration.Devices.OperationExists;
using Senswave.Integration.Shared;

namespace Senswave.Devices.Infrastructure.Operations.Consumers;

public class OperationExists(
    ILogger<OperationExists> logger,
    IOperationQueryRepository queryRepository)
    : IConsumer<OperationExistsRequest>
{
    public async Task Consume(ConsumeContext<OperationExistsRequest> context)
    {
        logger.LogInformation("[OperationExists] Received operation exists request");

        var operation = await queryRepository.GetOperation(context.Message.OperationId, context.CancellationToken);

        if (operation is null)
        {
            await context.RespondAsync(new OperationExistsResponse() { StatusCode = InternalRequestStatus.Failure });
        }
        else
        {
            await context.RespondAsync(new OperationExistsResponse());
        }
    }
}