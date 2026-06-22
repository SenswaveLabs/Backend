using Microsoft.EntityFrameworkCore;
using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.ShareDevices.Entites;
using Senswave.Devices.Domain.ShareDevices.Repositories;

namespace Senswave.Devices.Infrastructure.ShareDevices.Repositories;

internal sealed class DeviceSharingQueryRepository(DevicesContext context) : IDeviceSharingQueryRepository
{
    public Task<Device?> GetDevice(Guid deviceId, CancellationToken cancellationToken) => context.Devices
        .Where(x => x.Id == deviceId)
        .Include(x => x.HomeReference)
        .AsNoTracking()
        .FirstOrDefaultAsync(cancellationToken);

    public Task<Guid> GetHomeReferenceIdByDeviceId(Guid deviceId, CancellationToken cancellationToken) => context.Devices
        .Where(x => x.Id == deviceId)
        .AsNoTracking()
        .Select(x => x.HomeReference.HomeId)
        .FirstOrDefaultAsync(cancellationToken);

    public Task<List<DeviceSharing>> GetSharingsByDeviceId(Guid deviceId, CancellationToken cancellationToken) => context.DeviceSharings
        .Include(x => x.Device)
        .AsNoTracking()
        .Where(x => x.Device.Id == deviceId)
        .ToListAsync(cancellationToken);
}
