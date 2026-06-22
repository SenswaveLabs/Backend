using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Operations.Entities;

namespace Senswave.Devices.Domain.Operations.Repositories;

public interface IOperationQueryRepository
{
    Task<Operation?> GetOperation(Guid operationId, CancellationToken cancellationToken);

    Task<List<Operation>> GetOperations(Guid deviceId, int page, int size, CancellationToken cancellationToken);

    Task<List<Device>> GetDevicesWithSharingsByOperations(ISet<Guid> operationIds, CancellationToken cancellationToken);

    Task<List<Operation>> GetOperationsByIds(ISet<Guid> operationIds, CancellationToken cancellationToken);

    Task<bool> OperationHasWidget(Guid operationId, CancellationToken cancellationToken);

    Task<bool> OperationHasDeviceTile(Guid operationId, CancellationToken cancellationToken);

    Task<Guid> GetDeviceIdByOperationId(Guid operationId, CancellationToken cancellationToken);

    Task<int> CountOperationsByDevice(Guid deviceId, CancellationToken cancellationToken);
}
