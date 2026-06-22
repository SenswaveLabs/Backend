namespace Senswave.Devices.Domain.Dashboards.Services;

public interface IDashboardAccessService
{
    Task<Result> CanDisplay(Guid userId, Guid dashboardId, CancellationToken cancellationToken);
    Task<Result> CanDisplayDevice(Guid userId, Guid deviceId, CancellationToken cancellationToken);
    Task<Result> CanManage(Guid userId, Guid dashboardId, CancellationToken cancellationToken);
    Task<Result> CanManageDevice(Guid userId, Guid deviceId, CancellationToken cancellationToken);
    Task<Result> IsOwner(Guid userId, Guid dashboardId, CancellationToken cancellationToken);
}
