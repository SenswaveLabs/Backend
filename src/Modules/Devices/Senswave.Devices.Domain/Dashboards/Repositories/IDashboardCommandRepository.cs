using Senswave.Devices.Domain.Dashboards.Entities;

namespace Senswave.Devices.Domain.Dashboards.Repositories;

public interface IDashboardCommandRepository
{
    Task<Result> CreateDashboard(Dashboard dashboard, CancellationToken cancellationToken);
    Task<Result> UpdateDashboard(Dashboard dashboard, CancellationToken cancellationToken);
    Task<Result> DeleteDashboard(Guid dashboardId, CancellationToken cancellationToken);
    Task<Dashboard?> GetDashboard(Guid dashboardId, CancellationToken cancellationToken);
}