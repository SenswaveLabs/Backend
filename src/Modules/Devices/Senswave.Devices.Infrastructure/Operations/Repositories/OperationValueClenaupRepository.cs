using Microsoft.EntityFrameworkCore;
using Senswave.Devices.Domain.Operations.Repositories;

namespace Senswave.Devices.Infrastructure.Operations.Repositories;

public class OperationValueClenaupRepository(DevicesContext context,
    ILogger<OperationValueClenaupRepository> logger)
    : IOperationCleanupRepository
{
    public const int DefaultNumberOfOperationValuesToKeep = 100;

    public Task<int> RemoveOperationValues(Guid operationId, int skip = DefaultNumberOfOperationValuesToKeep, CancellationToken cancellationToken = default)
    {
        try
        {
            return context.Operations
                .Where(x => x.Id == operationId)
                .SelectMany(x => x.Values)
                .OrderByDescending(x => x.ProcessedAtUtc)
                .Skip(skip)
                .AsNoTracking()
                .ExecuteDeleteAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to remove operation values for operation {operationId}", operationId);
            return Task.FromResult(0);
        }
    }

    public async Task<int> RemoveOperationValues(List<Guid> operations, int skip = DefaultNumberOfOperationValuesToKeep, CancellationToken cancellationToken = default)
    {
        int counter = 0;
        foreach (var operationId in operations)
        {
            counter += await RemoveOperationValues(operationId, skip, cancellationToken);
        }

        return counter;
    }
}
