namespace Senswave.Devices.Domain.Operations.Repositories;

public interface IOperationCleanupRepository
{
    Task<int> RemoveOperationValues(Guid operationId, int skip = 100, CancellationToken cancellationToken = default);
    Task<int> RemoveOperationValues(List<Guid> operations, int skip = 100, CancellationToken cancellationToken = default);
}
