using Microsoft.EntityFrameworkCore;
using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Widgets.Entities;
using Senswave.Devices.Domain.Widgets.Repositories;

namespace Senswave.Devices.Infrastructure.Widgets.Repositories;

internal class WidgetQueryRepository(DevicesContext context) : IWidgetQueryRepository
{
    public Task<Widget?> GetWidgetWithOperation(Guid widgetId, CancellationToken cancellationToken) => context.Widgets
        .Where(x => x.Id == widgetId)
        .Include(x => x.Operation!)
            .ThenInclude(x => x.Values)
        .AsNoTracking()
        .FirstOrDefaultAsync(cancellationToken);

    public Task<List<Guid>> GetWidgetsByOperationId(Guid operationId, CancellationToken cancellationToken) => context.Widgets
        .Where(x => x.Operation!.Id == operationId)
        .AsNoTracking()
        .Select(x => x.Id)
        .ToListAsync(cancellationToken);

    public Task<Guid> GetDeviceIdByOperations(List<Guid> operationIds, CancellationToken cancellationToken) => context.Operations
        .Where(x => operationIds.Contains(x.Id))
        .AsNoTracking()
        .Select(x => x.Device!.Id)
        .FirstOrDefaultAsync(cancellationToken);

    public Task<List<Guid>> GetWidgetsByOperationIds(List<Guid> operationIds, CancellationToken cancellationToken) => context.Widgets
        .Where(x => operationIds.Contains(x.Operation!.Id))
        .AsNoTracking()
        .Select(x => x.Id)
        .ToListAsync(cancellationToken);

    public Task<Guid> GetDeviceIdByWidgetId(Guid widgetId, CancellationToken cancellationToken) => context.Widgets
        .Where(x => x.Id == widgetId)
        .AsNoTracking()
        .Select(x => x.Operation!.Device!.Id)
        .FirstOrDefaultAsync(cancellationToken);

    public Task<List<Operation>> GetOperationWithWidgets(Guid deviceId, CancellationToken cancellationToken) => context.Operations
        .Include(x => x.Widgets)
        .Where(x => x.DeviceId == deviceId)
        .AsNoTracking()
        .ToListAsync(cancellationToken);

    public Task<List<Widget>> GetWidgetsWithOperation(List<Guid> widgetIds, CancellationToken cancellationToken) => context.Widgets
        .Where(x => widgetIds.Contains(x.Id))
        .Include(x => x.Operation)
        .AsNoTracking()
        .ToListAsync(cancellationToken);
}