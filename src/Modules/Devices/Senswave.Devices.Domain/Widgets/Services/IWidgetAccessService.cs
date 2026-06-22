namespace Senswave.Devices.Domain.Widgets.Services;

public interface IWidgetAccessService
{
    Task<Result> CanAct(Guid userId, Guid widgetId, CancellationToken cancellationToken);
    Task<Result> CanDisplay(Guid userId, Guid widgetId, CancellationToken cancellationToken);
    Task<Result> CanDisplayDevice(Guid userId, Guid dashboardId, CancellationToken cancellationToken);
    Task<Result> CanManage(Guid userId, Guid widgetId, CancellationToken cancellationToken);
    Task<Result> CanManageDevice(Guid userId, Guid dashboardId, CancellationToken cancellationToken);
    Task<Result> IsOwner(Guid userId, Guid widgetId, CancellationToken cancellationToken);
}
