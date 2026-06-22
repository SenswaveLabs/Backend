using Senswave.Abstractions.Resulting;
using Senswave.Devices.Domain.Dashboards.Entities;
using Senswave.Devices.Domain.Dashboards.Extensions;

namespace Senswave.Devices.Api.Dashboards.DisplayDashboards;

internal static class DisplayDashboardsExtensions
{
    internal static DisplayDashboardsResponse ToResponse(this Result<List<Dashboard>> result) => new()
    {
        Items = [.. result.Data.Select(x => x.ToDto())]
    };

    public static DashboardDto ToDto(this Dashboard dashboard) => new()
    {
        Id = dashboard.Id,
        Name = dashboard.Name,
        Icon = dashboard.Icon,
        Type = dashboard.Type.FromDashboardType()
    };
}
