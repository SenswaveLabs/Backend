using Microsoft.EntityFrameworkCore;
using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Devices.Repositories;
using Senswave.Devices.Domain.ShareDevices.Enums;

namespace Senswave.Devices.Infrastructure.Devices.Repositories;

internal class DevicesQueryRepository(DevicesContext context) : IDeviceQueryRepository
{
    public Task<Device?> GetDeviceByName(Guid homeId, string name, CancellationToken cancellationToken) => context.Devices
        .Where(x => x.HomeReference.HomeId == homeId)
        .Where(x => x.Name == name)
        .AsNoTracking()
        .FirstOrDefaultAsync(cancellationToken);

    public Task<int> CountDevicesForHome(Guid homeId, CancellationToken cancellationToken) => context.Devices
        .Where(x => x.HomeReference.HomeId == homeId)
        .AsNoTracking()
        .CountAsync(cancellationToken);

    public Task<Device?> GetDevice(Guid deviceId, CancellationToken cancellationToken) => context.Devices
        .Where(x => x.Id == deviceId)
        .Include(x => x.Tile)
            .ThenInclude(x => x.SwitchOperation)
        .Include(x => x.Tile)
            .ThenInclude(x => x.DisplayableOperation)
        .Include(x => x.Presence)
            .ThenInclude(x => x!.Operation)
        .AsNoTracking()
        .FirstOrDefaultAsync(cancellationToken);

    public Task<List<Device>> GetDevices(Guid homeId, int page, int size, CancellationToken cancellationToken) => context.Devices
        .Where(x => x.HomeReference.HomeId == homeId)
        .OrderBy(x => x.Name)
        .Skip((page - 1) * size)
        .Take(size)
        .Include(x => x.Tile)
            .ThenInclude(x => x.SwitchOperation)
        .Include(x => x.Tile)
            .ThenInclude(x => x.DisplayableOperation)
        .Include(x => x.Presence)
            .ThenInclude(x => x!.Operation)
        .AsNoTracking()
        .ToListAsync(cancellationToken);

    public Task<List<Device>> GetDevices(Guid homeId, CancellationToken cancellationToken) => context.Devices
        .Where(x => x.HomeReference.HomeId == homeId)
        .OrderBy(x => x.Name)
        .Include(x => x.Tile)
            .ThenInclude(x => x.SwitchOperation)
        .Include(x => x.Tile)
            .ThenInclude(x => x.DisplayableOperation)
        .Include(x => x.Presence)
            .ThenInclude(x => x!.Operation)
        .AsNoTracking()
        .ToListAsync(cancellationToken);

    public Task<List<Guid>> GetDevicesByHome(Guid homeId, CancellationToken cancellationToken) => context.Devices
        .Where(x => x.HomeReference.HomeId == homeId)
        .AsNoTracking()
        .Select(x => x.Id)
        .ToListAsync(cancellationToken);

    public Task<Device?> GetDeviceByOperationsIfTileOperationAction(List<Guid> operationIds, CancellationToken cancellationToken) => context.Devices
        .Where(x => x.Tile != null)
        .Where(x => x.Tile!.SwitchOperation != null)
        .Where(x => operationIds.Any(y => y == x.Tile!.SwitchOperation!.Id))
        .AsNoTracking()
        .FirstOrDefaultAsync(cancellationToken);

    public Task<Guid> GetHomeIdByDevice(Guid deviceId, CancellationToken cancellationToken) => context.Devices
        .Where(x => x.Id == deviceId)
        .AsNoTracking()
        .Select(x => x.HomeReference.HomeId)
        .FirstOrDefaultAsync(cancellationToken);

    public Task<bool> IsDeviceOwner(Guid userId, Guid deviceId, CancellationToken cancellationToken) => context.Devices
        .Where(x => x.HomeReference.OwnerId == userId)
        .Where(x => x.Id == deviceId)
        .AsNoTracking()
        .AnyAsync(cancellationToken);

    public Task<bool> IsDeviceShared(Guid userId, Guid deviceId, CancellationToken cancellationToken) => context.DeviceSharings
        .Where(x => x.UserId == userId)
        .Where(x => x.Device!.Id == deviceId)
        .AsNoTracking()
        .AnyAsync(cancellationToken);

    public Task<bool> HasDisplayAccessToDevice(Guid userId, Guid deviceId, CancellationToken cancellationToken) => context.DeviceSharings
        .Where(x => x.UserId == userId)
        .Where(x => x.Device!.Id == deviceId)
        .Where(x => x.SharingType >= DeviceSharingType.Display)
        .AsNoTracking()
        .AnyAsync(cancellationToken);

    public Task<bool> HasActionAccessToDevice(Guid userId, Guid deviceId, CancellationToken cancellationToken) => context.DeviceSharings
        .Where(x => x.UserId == userId)
        .Where(x => x.Device!.Id == deviceId)
        .Where(x => x.SharingType >= DeviceSharingType.Action)
        .AsNoTracking()
        .AnyAsync(cancellationToken);

    public Task<bool> HasManageAccessToDevice(Guid userId, Guid deviceId, CancellationToken cancellationToken) => context.DeviceSharings
        .Where(x => x.UserId == userId)
        .Where(x => x.Device!.Id == deviceId)
        .Where(x => x.SharingType >= DeviceSharingType.Manage)
        .AsNoTracking()
        .AnyAsync(cancellationToken);

    public Task<bool> DeviceNameUsedInHome(Guid homeId, string name, CancellationToken cancellationToken) => context.Devices
        .Where(x => x.HomeReference.HomeId == homeId)
        .Where(x => x.Name == name)
        .AsNoTracking()
        .AnyAsync(cancellationToken);

    public Task<Guid> GetDeviceIdByWidgetId(Guid widgetId, CancellationToken cancellationToken) => context.Widgets
        .Where(x => x.Id == widgetId)
        .Select(x => x.Operation!)
        .Select(x => x.Device!.Id)
        .FirstOrDefaultAsync(cancellationToken);

    public Task<Guid> GetDeviceIdByOperationId(Guid operationId, CancellationToken cancellationToken) => context.Operations
        .Where(x => x.Id == operationId)
        .Select(x => x.Device!.Id)
        .FirstOrDefaultAsync(cancellationToken);

    public Task<Device?> GetDeviceForOnlineStatus(Guid deviceId, CancellationToken cancellationToken) => context.Devices
        .Where(x => x.Id == deviceId)
        .Include(x => x.Presence)
            .ThenInclude(x => x!.Operation)
        .AsNoTracking()
        .FirstOrDefaultAsync(cancellationToken);

    public Task<Device?> GetDeviceIdByOperationIds(List<Guid> operationIds, CancellationToken cancellationToken) => context.DevicePresence
        .Where(x => operationIds.Any(y => y == x.Operation!.Id))
        .Select(x => x.Device!)
        .AsNoTracking()
        .FirstOrDefaultAsync(cancellationToken);
}
