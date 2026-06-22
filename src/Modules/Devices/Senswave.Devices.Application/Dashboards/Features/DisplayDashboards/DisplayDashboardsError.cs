namespace Senswave.Devices.Application.Dashboards.Features.DisplayDashboards;

internal class DisplayDashboardsError
{
    public static Error DashboardsNotFound => Error.NotFound("DashboardsNotFound", "Dashboards not found.");
}
