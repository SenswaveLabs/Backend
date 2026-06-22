using Senswave.Devices.Domain.Dashboards.Factories;
using Senswave.Devices.Domain.Dashboards.Repositories;
using Senswave.Devices.Domain.Dashboards.Services;
using Senswave.Devices.Domain.Dashboards.Types.Gird;

namespace Senswave.Devices.Application.Dashboards.Features.SetWidgetOnDashboard;

public class SetWidgetOnDashboardHandler(
    IDashboardsQueryRespository queryRespository,
    IDashboardCommandRepository commandRepository,
    IDashboardAccessService accessService,
    DashboardFactory dashboardFactory,
    ILogger<SetWidgetOnDashboardHandler> logger) : ICommandHandler<SetWidgetOnDashboardCommand>
{
    public async Task<Result> Handle(SetWidgetOnDashboardCommand request, CancellationToken cancellationToken)
    {
        var access = await accessService.CanManage(request.UserId, request.DashboardId, cancellationToken);

        if (access.IsFailure)
            return Result.Failure(access.Errors);

        var widgetExists = await queryRespository.WidgetExists(request.WidgetId, cancellationToken);

        if (!widgetExists)
        {
            logger.LogWarning("[UserId: {UserId}] attempted to set a non-existing widget: {WidgetId} on dashboard: {DashboardId}.",
                request.UserId,
                request.WidgetId,
                request.DashboardId);
            return Result.Failure(SetWidgetOnDashboardErrors.WidgetNotFound);
        }

        var validDeviceMembership = await queryRespository.ValidateDeviceMembership(request.WidgetId, request.DashboardId, cancellationToken);

        if (!validDeviceMembership)
        {
            logger.LogWarning("[UserId: {UserId}] attempted to set widget: {WidgetId} on dashboard: {DashboardId} without valid device membership.",
                request.UserId,
                request.WidgetId,
                request.DashboardId);
            return Result.Failure(SetWidgetOnDashboardErrors.InvalidDeviceMembership);
        }

        //TODO: Redis Lock

        var dashboardResult = await dashboardFactory.Create(request.DashboardId, cancellationToken);

        if (dashboardResult.IsFailure)
        {
            logger.LogError("[UserId: {UserId}] failed to create dashboard for widget placement: {DashboardId}.",
                request.UserId,
                request.DashboardId);
            return Result.Failure(dashboardResult.Errors);
        }

        var positionedWidget = new PositionedWidget
        {
            WidgetId = request.WidgetId,
            Row = request.Row,
            RowSpan = request.RowSpan,
            Column = request.Column,
            ColumnSpan = request.ColumnSpan
        };

        var validationResult = dashboardResult.Data.AddWidget(positionedWidget);

        if (validationResult.IsFailure)
        {
            logger.LogWarning("[UserId: {UserId}] failed to add widget: {WidgetId} to dashboard: {DashboardId}.",
                request.UserId,
                request.WidgetId,
                request.DashboardId);
            return Result.Failure(validationResult.Errors);
        }

        var dashboard = dashboardResult.Data.ToDashboard();

        var updateResult = await commandRepository.UpdateDashboard(dashboard, cancellationToken);

        if (updateResult.IsFailure)
        {
            logger.LogError("[UserId: {UserId}] failed to update dashboard: {DashboardId} with widget: {WidgetId}.",
                request.UserId,
                request.DashboardId,
                request.WidgetId);
            return Result.Failure(updateResult.Errors);
        }

        logger.LogInformation("[UserId: {UserId}] successfully set widget: {WidgetId} on dashboard: {DashboardId}.",
            request.UserId,
            request.WidgetId,
            request.DashboardId);
        return Result.Success();
    }
}
