using Senswave.Devices.Domain.Dashboards.Entities;

namespace Senswave.Devices.Domain.Dashboards.Repositories;

public interface IDashboardsQueryRespository
{
    Task<List<Dashboard>> GetDashboards(Guid deviceId, CancellationToken cancellation);
    Task<Dashboard?> GetDashboard(Guid dashboardId, CancellationToken cancellation);
    Task<bool> NameUsed(Guid deviceId, string dashboardName, CancellationToken cancellationToken);
    Task<Guid> GetDeviceIdByDashboardId(Guid dashboardId, CancellationToken cancellationToken);
    Task<int> CountDashboardsByDevice(Guid deviceId, CancellationToken cancellationToken);
    Task<bool> WidgetExists(Guid widgetId, CancellationToken cancellationToken);
    Task<bool> ValidateDeviceMembership(Guid widgetId, Guid dashboardId, CancellationToken cancellationToken);
}
