
using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.ShareDevices.Entites;

namespace Senswave.Devices.Domain.ShareDevices.Repositories;

public interface IDeviceSharingQueryRepository
{
    Task<Device?> GetDevice(Guid deviceId, CancellationToken cancellationToken);
    Task<Guid> GetHomeReferenceIdByDeviceId(Guid deviceId, CancellationToken cancellationToken);
    public Task<List<DeviceSharing>> GetSharingsByDeviceId(Guid deviceId, CancellationToken cancellationToken);
}
