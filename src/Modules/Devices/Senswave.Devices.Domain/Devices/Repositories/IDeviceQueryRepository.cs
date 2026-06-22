using Senswave.Devices.Domain.Devices.Entities;

namespace Senswave.Devices.Domain.Devices.Repositories;

public interface IDeviceQueryRepository
{
    Task<Guid> GetDeviceIdByOperationId(Guid operationId, CancellationToken cancellationToken);

    Task<Guid> GetDeviceIdByWidgetId(Guid widgetId, CancellationToken cancellationToken);

    Task<Device?> GetDeviceForOnlineStatus(Guid deviceId, CancellationToken cancellationToken);

    Task<bool> IsDeviceOwner(Guid userId, Guid deviceId, CancellationToken cancellationToken);

    Task<bool> IsDeviceShared(Guid userId, Guid deviceId, CancellationToken cancellationToken);

    Task<bool> HasDisplayAccessToDevice(Guid userId, Guid deviceId, CancellationToken cancellationToken);

    Task<bool> HasManageAccessToDevice(Guid userId, Guid deviceId, CancellationToken cancellationToken);

    Task<bool> HasActionAccessToDevice(Guid userId, Guid deviceId, CancellationToken cancellationToken);

    Task<Guid> GetHomeIdByDevice(Guid deviceId, CancellationToken cancellationToken);

    Task<bool> DeviceNameUsedInHome(Guid homeId, string name, CancellationToken cancellationToken);

    Task<List<Device>> GetDevices(Guid homeId, int page, int size, CancellationToken cancellationToken);

    Task<List<Device>> GetDevices(Guid homeId, CancellationToken cancellationToken);

    Task<Device?> GetDevice(Guid deviceId, CancellationToken cancellationToken);

    Task<Device?> GetDeviceByName(Guid homeId, string name, CancellationToken cancellationToken);

    Task<int> CountDevicesForHome(Guid homeId, CancellationToken cancellationToken);

    Task<List<Guid>> GetDevicesByHome(Guid homeId, CancellationToken cancellationToken);

    Task<Device?> GetDeviceByOperationsIfTileOperationAction(List<Guid> operationIds, CancellationToken cancellationToken);

    Task<Device?> GetDeviceIdByOperationIds(List<Guid> operationIds, CancellationToken cancellationToken);
}
