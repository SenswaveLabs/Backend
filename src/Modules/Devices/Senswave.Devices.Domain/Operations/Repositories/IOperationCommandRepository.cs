using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Operations.ValueObjects;

namespace Senswave.Devices.Domain.Operations.Repositories;

public interface IOperationCommandRepository
{
    Task<List<Operation>> GetOperationsByReference(Guid subscribtionReferenceId, CancellationToken cancellationToken);

    Task<Operation?> GetOperation(Guid operationId, CancellationToken cancellationToken);

    Task<bool> UpdateOperationsWithValue(List<Operation> operations, CancellationToken cancellationToken);

    Task<bool> UpdateOperationWithValue(Operation operation, OperationValue value, CancellationToken cancellationToken);

    Task<Result> CreateOperation(Guid deviceId, Operation operation, CancellationToken cancellationToken);

    Task<Result> DeleteOperation(Guid operationId, CancellationToken cancellationToken);

    Task<Device?> GetDevice(Guid deviceId, CancellationToken cancellationToken);

    Task<DataSourceDataReference?> GetDataSourceDataReference(Guid subscriptionId, CancellationToken cancellationToken);
}