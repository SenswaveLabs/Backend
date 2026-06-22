using Senswave.Abstractions.Resulting;
using Senswave.Devices.Domain.Dashboards.Entities;
using Senswave.Devices.Domain.Dashboards.Extensions;

namespace Senswave.Devices.Api.Dashboards.GetDashboard;

internal static class GetDashboardExtensions
{
    public static GetDashboardResponse ToResponse(this Result<Dashboard> result) => new()
    {
        Id = result.Data.Id,
        Name = result.Data.Name,
        Icon = result.Data.Icon,
        Configuration = result.Data.Configuration,
        Type = result.Data.Type.FromDashboardType()
    };
}
