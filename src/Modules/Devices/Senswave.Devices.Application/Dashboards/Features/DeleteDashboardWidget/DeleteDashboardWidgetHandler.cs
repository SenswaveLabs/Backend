using Senswave.Devices.Domain.Dashboards.Factories;
using Senswave.Devices.Domain.Dashboards.Repositories;
using Senswave.Devices.Domain.Dashboards.Services;

namespace Senswave.Devices.Application.Dashboards.Features.DeleteDashboardWidget;

internal sealed class DeleteDashboardWidgetHandler(
    IDashboardAccessService accessService,
    IDashboardCommandRepository commandRepository,
    DashboardFactory factory,
    ILogger<DeleteDashboardWidgetHandler> logger) : ICommandHandler<DeleteDashboardWidgetCommand>
{
    public async Task<Result> Handle(DeleteDashboardWidgetCommand request, CancellationToken cancellationToken)
    {
        var access = await accessService.CanManage(request.UserId, request.DashboardId, cancellationToken);

        if (!access)
            return Result.Failure(access.Errors);

        // TODO: Redis Lock

        var dashboardResult = await factory.Create(request.DashboardId, cancellationToken);

        if (dashboardResult.IsFailure)
        {
            logger.LogWarning("[UserId: {UserId}][DashboardId: {DashboardId}] Dashboard not found for widget removal.", request.UserId, request.DashboardId);
            return Result.Failure(dashboardResult.Errors);
        }

        var removal = dashboardResult.Data.RemoveWidget(request.WidgetId);

        if (removal.IsFailure)
        {
            logger.LogWarning("[UserId: {UserId}][DashboardId: {DashboardId}] Failed to remove widget from dashboard.", request.UserId, request.DashboardId);
            return Result.Failure(removal.Errors);
        }

        var dashboard = dashboardResult.Data.ToDashboard();

        var update = await commandRepository.UpdateDashboard(dashboard, cancellationToken);

        if (update.IsFailure)
        {
            logger.LogError("[UserId: {UserId}][DashboardId: {DashboardId}] Failed to update dashboard after widget removal.", request.UserId, request.DashboardId);
            return Result.Failure(update.Errors);
        }

        logger.LogInformation("[UserId: {UserId}][DashboardId: {DashboardId}] Widget removed successfully from dashboard.", request.UserId, request.DashboardId);
        return Result.Success();
    }
}
