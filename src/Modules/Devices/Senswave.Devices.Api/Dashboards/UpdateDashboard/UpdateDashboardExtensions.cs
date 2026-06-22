using Senswave.Devices.Application.Dashboards.Features.UpdateDashboard;

namespace Senswave.Devices.Api.Dashboards.UpdateDashboard;

internal static class UpdateDashboardExtensions
{
    public static UpdateDashboardCommand ToCommand(this UpdateDashboardRequest dto, Guid userId, Guid dashboardId) => new()
    {
        DashboardId = dashboardId,
        UserId = userId,
        Name = dto.Name,
        Icon = dto.Icon,
    };
}
