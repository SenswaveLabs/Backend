using Senswave.Devices.Domain.Dashboards.Entities;
using Senswave.Devices.Domain.Dashboards.Repositories;
using Senswave.Devices.Domain.Dashboards.Services;

namespace Senswave.Devices.Application.Dashboards.Features.UpdateDashboard;

public class UpdateDashboardHandler(
    IDashboardAccessService accessService,
    IDashboardsQueryRespository dashboardsQueryRespository,
    IDashboardCommandRepository commandRepository,
    ILogger<UpdateDashboardHandler> logger) : ICommandHandler<UpdateDashboardCommand>
{
    public async Task<Result> Handle(UpdateDashboardCommand request, CancellationToken cancellationToken)
    {
        var canManage = await accessService.CanManage(request.UserId, request.DashboardId, cancellationToken);

        if (!canManage)
            return Result.Failure(canManage.Errors);

        var dashboard = await commandRepository.GetDashboard(request.DashboardId, cancellationToken);

        if (dashboard is null)
        {
            logger.LogWarning("[UserId: {UserId}][DashboardId: {DashboardId}] Dashboard not found for update.", request.UserId, request.DashboardId);
            return Result.Failure(UpdateDashboardErrors.DashboardNotFound);
        }

        var nameChangeStatus = await NameToChange(dashboard, request, cancellationToken);

        if (nameChangeStatus.IsFailure)
        {
            logger.LogWarning("[UserId: {UserId}][DashboardId: {DashboardId}] Failed to update dashboard name:",
                request.UserId,
                request.DashboardId);
            return nameChangeStatus;
        }

        if (!string.IsNullOrEmpty(request.Icon))
            dashboard.Icon = request.Icon;

        var result = await commandRepository.UpdateDashboard(dashboard, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogError("[UserId: {UserId}][DashboardId: {DashboardId}] Failed to update dashboard",
                request.UserId,
                request.DashboardId);
            return Result.Failure(result.Errors);
        }

        logger.LogInformation("[UserId: {UserId}][DashboardId: {DashboardId}] Dashboard updated successfully.",
            request.UserId,
            request.DashboardId);
        return Result.Success();
    }

    private async Task<Result> NameToChange(Dashboard dashboard, UpdateDashboardCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(command.Name))
            return Result.Success();

        if (dashboard.Name == command.Name)
            return Result.Success();

        var dashboardExists = await dashboardsQueryRespository
            .NameUsed(dashboard.DeviceId, command.Name, cancellationToken);

        if (dashboardExists)
            return Result.Failure(UpdateDashboardErrors.DashboardNameHasAlreadyExists);

        dashboard.Name = command.Name;
        return Result.Success();
    }
}