using Senswave.Devices.Domain.Dashboards.Entities;
using Senswave.Devices.Domain.Dashboards.Repositories;
using Senswave.Devices.Domain.Dashboards.Services;

namespace Senswave.Devices.Application.Dashboards.Features.DisplayDashboards;

internal class DisplayDashboardsHandler(
    IDashboardAccessService accessService,
    IDashboardsQueryRespository repository) : IQueryHandler<DisplayDashboardsQuery, List<Dashboard>>
{
    public async Task<Result<List<Dashboard>>> Handle(DisplayDashboardsQuery request, CancellationToken cancellationToken)
    {
        var canDisplay = await accessService.CanDisplayDevice(request.UserId, request.DeviceId, cancellationToken);

        if (!canDisplay)
            return Result<List<Dashboard>>.Failure(canDisplay.Errors);

        var dashboards = await repository.GetDashboards(request.DeviceId, cancellationToken);

        if (dashboards.Count == 0)
            return Result<List<Dashboard>>.Failure(DisplayDashboardsError.DashboardsNotFound);

        return Result<List<Dashboard>>.Success(dashboards);
    }
}
