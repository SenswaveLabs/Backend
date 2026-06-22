using Senswave.Devices.Domain.Dashboards.Entities;
using Senswave.Devices.Domain.Dashboards.Repositories;
using Senswave.Devices.Domain.Dashboards.Services;

namespace Senswave.Devices.Application.Dashboards.Features.GetDashboard;

internal class GetDashboardHandler(
    IDashboardAccessService accessService,
    IDashboardsQueryRespository repository,
    ILogger<GetDashboardHandler> logger) : IQueryHandler<GetDashboardQuery, Dashboard>
{
    public async Task<Result<Dashboard>> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var canDisplay = await accessService.CanDisplay(request.UserId, request.DashboardId, cancellationToken);

        if (!canDisplay)
            return Result<Dashboard>.Failure(canDisplay.Errors);

        var dashboard = await repository.GetDashboard(request.DashboardId, cancellationToken);

        if (dashboard is null)
        {
            logger.LogWarning("[UserId: {UserId}][DashboardId: {DashboardId}] Dashboard not found.", request.UserId, request.DashboardId);
            return Result<Dashboard>.Failure(GetDashboardErrors.DashboardNotFound);
        }

        logger.LogInformation("[UserId: {UserId}][DashboardId: {DashboardId}] Dashboard retrieved successfully.", request.UserId, request.DashboardId);
        return Result<Dashboard>.Success(dashboard);
    }
}
