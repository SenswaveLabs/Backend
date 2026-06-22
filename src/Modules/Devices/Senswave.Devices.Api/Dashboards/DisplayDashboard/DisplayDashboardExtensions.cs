using Senswave.Abstractions.Resulting;
using Senswave.Devices.Domain.Dashboards.Extensions;
using Senswave.Devices.Domain.Dashboards.Models;

namespace Senswave.Devices.Api.Dashboards.DisplayDashboard;

internal static class DisplayDashboardExtensions
{
    internal static DisplayDashboardResponse ToDisplayDashboardResponse(this Result<DisplayDashboardModel> result) => new()
    {
        Type = result.Data.Type.FromDashboardType(),
        Configuration = result.Data.Configuration
    };
}
