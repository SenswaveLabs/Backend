using Microsoft.EntityFrameworkCore;
using Senswave.Devices.Domain.Dashboards.Entities;
using Senswave.Devices.Domain.Dashboards.Repositories;

namespace Senswave.Devices.Infrastructure.Dashboards.Repositories;

internal class DashboardsQueryRepository(DevicesContext context) : IDashboardsQueryRespository
{
    public Task<List<Dashboard>> GetDashboards(Guid deviceId, CancellationToken cancellation) => context.Dashboards
        .Where(x => x.Device!.Id == deviceId)
        .AsNoTracking()
        .ToListAsync(cancellation);

    public Task<Dashboard?> GetDashboard(Guid dashboardId, CancellationToken cancellation) => context.Dashboards
        .Where(x => x.Id == dashboardId)
        .AsNoTracking()
        .FirstOrDefaultAsync(cancellation);

    public async Task<bool> NameUsed(Guid deviceId, string dashboardName, CancellationToken cancellationToken) => await context.Dashboards
        .Where(x => x.Device!.Id == deviceId)
        .Where(x => x.Name == dashboardName)
        .AsNoTracking()
        .AnyAsync(cancellationToken);

    public Task<Guid> GetDeviceIdByDashboardId(Guid dashboardId, CancellationToken cancellationToken) => context.Dashboards
        .Where(x => x.Id == dashboardId)
        .Include(x => x.Device)
        .AsNoTracking()
        .Select(x => x.DeviceId)
        .FirstOrDefaultAsync(cancellationToken);

    public Task<int> CountDashboardsByDevice(Guid deviceId, CancellationToken cancellationToken) => context.Dashboards
        .Where(x => x.DeviceId == deviceId)
        .CountAsync(cancellationToken);

    public Task<bool> WidgetExists(Guid widgetId, CancellationToken cancellationToken) => context.Widgets
        .AsNoTracking()
        .Where(x => x.Id == widgetId)
        .AnyAsync(cancellationToken);

    public async Task<bool> ValidateDeviceMembership(Guid widgetId, Guid dashboardId, CancellationToken cancellationToken)
    {
        var dashboard = await context.Dashboards
            .Where(x => x.Id == dashboardId)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (dashboard is null)
            return false;

        return await context.Widgets
            .Where(x => x.Operation!.DeviceId == dashboard.DeviceId)
            .Where(x => x.Id == widgetId)
            .AsNoTracking()
            .AnyAsync(cancellationToken);
    }
}
