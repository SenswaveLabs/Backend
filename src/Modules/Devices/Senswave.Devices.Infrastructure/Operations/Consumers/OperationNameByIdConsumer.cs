using Senswave.Devices.Domain.Operations.Repositories;
using Senswave.Integration.Devices.OperationNameById;

namespace Senswave.Devices.Infrastructure.Operations.Consumers;

public class OperationNameByIdConsumer(
    IOperationQueryRepository queryRepository,
    ILogger<OperationNameByIdConsumer> logger)
    : IConsumer<OperationNameByIdRequest>
{
    public async Task Consume(ConsumeContext<OperationNameByIdRequest> context)
    {
        var operations = await queryRepository.GetOperationsByIds(
            context.Message.OperationIds.ToHashSet(), context.CancellationToken);

        var guidToName = new Dictionary<Guid, string>();
        foreach (var operation in operations)
        {
            guidToName.Add(operation.Id, operation.Name);
        }

        logger.LogInformation("Operation names by IDs request processed successfully. Count: {Count}", guidToName.Count);
        await context.RespondAsync(new OperationNameByIdsResponse
        {
            IdToName = guidToName
        });
    }
}