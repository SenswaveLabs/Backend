using Senswave.Devices.Domain.Devices.Entities;

namespace Senswave.Devices.Domain.Devices.Repositories;

public interface IDeviceCommandRepository
{
    Task<Result> CreateDevice(Guid homeId, Guid homeOwnerId, Device device, CancellationToken cancellationToken);
    Task<Result> UpdateDevice(Device device, CancellationToken cancellationToken);
    Task<Result> UpdateDeviceWithAddingPresence(Device device, CancellationToken cancellationToken);
    Task<Result> DeleteDevice(Device device, CancellationToken cancellationToken);
    Task<Device?> GetDeviceWithTile(Guid deviceId, CancellationToken cancellationToken);
    Task<Device?> GetDeviceForDeletion(Guid deviceId, CancellationToken cancellationToken);
}