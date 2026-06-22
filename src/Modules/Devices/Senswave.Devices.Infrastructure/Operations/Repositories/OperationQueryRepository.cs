using Microsoft.EntityFrameworkCore;
using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Operations.Repositories;

namespace Senswave.Devices.Infrastructure.Operations.Repositories;

internal class OperationRepository(DevicesContext context) : IOperationQueryRepository
{
    public Task<Operation?> GetOperation(Guid operationId, CancellationToken cancellationToken) => context.Operations
        .Where(o => o.Id == operationId)
        .Include(x => x.Device!)
            .ThenInclude(d => d.HomeReference)
        .Include(x => x.Values)
        .Include(x => x.DataReference)
        .AsNoTracking()
        .FirstOrDefaultAsync(cancellationToken);

    public Task<List<Operation>> GetOperations(Guid deviceId, int page, int size, CancellationToken cancellationToken) => context.Operations
        .Where(x => x.Device!.Id == deviceId)
        .Skip((page-1) * size)
        .Take(size)
        .AsNoTracking()
        .ToListAsync(cancellationToken);

    public Task<List<Device>> GetDevicesWithSharingsByOperations(ISet<Guid> operationIds, CancellationToken cancellationToken) => context.Operations
        .Where(x => operationIds.Contains(x.Id) && x.Device != null)
        .Include(x => x.Device!)
            .ThenInclude(x => x.DeviceSharings)
        .Select(x => x.Device!)
        .ToListAsync(cancellationToken);

    public Task<List<Operation>> GetOperationsByIds(ISet<Guid> operationIds, CancellationToken cancellationToken) => context.Operations
        .Where(x => operationIds.Contains(x.Id))
        .ToListAsync(cancellationToken);

    public Task<bool> OperationHasWidget(Guid operationId, CancellationToken cancellationToken) => context.Widgets
        .Where(x => x.Operation!.Id == operationId)
        .AnyAsync(cancellationToken);

    public Task<bool> OperationHasDeviceTile(Guid operationId, CancellationToken cancellationToken) => context.DeviceTiles
        .Where(x => x.SwitchOperationId == operationId)
        .AnyAsync(cancellationToken);

    public Task<Guid> GetDeviceIdByOperationId(Guid operationId, CancellationToken cancellationToken) => context.Operations
        .Where(x => x.Id == operationId)
        .Select(x => x.Device!.Id)
        .FirstOrDefaultAsync(cancellationToken);

    public Task<int> CountOperationsByDevice(Guid deviceId, CancellationToken cancellationToken) => context.Operations
        .Where(x => x.Device!.Id == deviceId)
        .CountAsync(cancellationToken);
}
