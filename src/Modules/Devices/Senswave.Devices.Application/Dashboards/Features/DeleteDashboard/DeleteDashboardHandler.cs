using Senswave.Devices.Domain.Dashboards.Entities;
using Senswave.Devices.Domain.Dashboards.Repositories;
using Senswave.Devices.Domain.Dashboards.Services;

namespace Senswave.Devices.Application.Dashboards.Features.DeleteDashboard;

public class DeleteDashboardHandler(
    IDashboardAccessService accessService,
    IDashboardCommandRepository commandRepository,
    ILogger<DeleteDashboardHandler> logger) : ICommandHandler<DeleteDashboardCommand>
{
    public async Task<Result> Handle(DeleteDashboardCommand request, CancellationToken cancellationToken)
    {
        var canManage = await accessService.CanManage(request.UserId, request.DashboardId, cancellationToken);

        if (!canManage)
            return Result<Dashboard>.Failure(canManage.Errors);

        var result = await commandRepository.DeleteDashboard(request.DashboardId, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogError("[UserId: {UserId}][DashboardId: {DashboardId}] Failed to delete dashboard.", request.UserId, request.DashboardId);
            return Result.Failure(result.Errors);
        }

        logger.LogInformation("[UserId: {UserId}][DashboardId: {DashboardId}] Dashboard deleted successfully.", request.UserId, request.DashboardId);
        return Result.Success();
    }
}