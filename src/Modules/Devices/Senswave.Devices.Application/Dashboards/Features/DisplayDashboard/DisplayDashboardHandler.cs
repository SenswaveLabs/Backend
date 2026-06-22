using Senswave.Devices.Domain.Dashboards.Factories;
using Senswave.Devices.Domain.Dashboards.Models;
using Senswave.Devices.Domain.Dashboards.Services;

namespace Senswave.Devices.Application.Dashboards.Features.DisplayDashboard;

internal sealed class DisplayDashboardHandler(
    DashboardFactory factory,
    IDashboardAccessService accessService,
    ILogger<DisplayDashboardHandler> logger) : IQueryHandler<DisplayDashboardQuery, DisplayDashboardModel>
{
    public async Task<Result<DisplayDashboardModel>> Handle(DisplayDashboardQuery request, CancellationToken cancellationToken)
    {
        var canDisplay = await accessService.CanDisplay(request.UserId, request.DashboardId, cancellationToken);

        if (!canDisplay)
            return Result<DisplayDashboardModel>.Failure(canDisplay.Errors);

        var dashbaord = await factory.Create(request.DashboardId, cancellationToken);

        if (dashbaord.IsFailure)
        {
            logger.LogWarning("[UserId: {UserId}][DashboardId: {DashboardId}] Dashboard not found for display.", request.UserId, request.DashboardId);
            return Result<DisplayDashboardModel>.Failure(dashbaord.Errors);
        }

        var response = await dashbaord.Data.ToDisplay(cancellationToken);

        if (response.IsFailure)
        {
            logger.LogError("[UserId: {UserId}][DashboardId: {DashboardId}] Failed to display dashboard.", request.UserId, request.DashboardId);
            return Result<DisplayDashboardModel>.Failure(response.Errors);
        }

        logger.LogInformation("[UserId: {UserId}][DashboardId: {DashboardId}] Dashboard displayed successfully.", request.UserId, request.DashboardId);
        return Result<DisplayDashboardModel>.Success(response.Data);
    }
}
