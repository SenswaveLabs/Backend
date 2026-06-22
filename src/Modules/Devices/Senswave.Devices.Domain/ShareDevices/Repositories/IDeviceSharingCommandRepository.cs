using Senswave.Devices.Domain.ShareDevices.Entites;
using Senswave.Devices.Domain.ShareDevices.Enums;

namespace Senswave.Devices.Domain.ShareDevices.Repositories;

public interface IDeviceSharingCommandRepository
{
    Task<DeviceSharing?> GetDeviceSharing(Guid deviceSharing, CancellationToken cancellationToken);

    Task<Result> DeleteDeviceSharing(DeviceSharing deviceSharing, CancellationToken cancellationToken);

    Task<Result> DeleteDevicesSharings(Guid userId, Guid homeReferenceId, CancellationToken cancellationToken);

    Task<Result> CreateOrUpdateDeviceSharing(Guid friendId, Guid deviceId, DeviceSharingType deviceSharing, CancellationToken cancellationToken);
}