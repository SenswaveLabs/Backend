namespace Senswave.Devices.Application.Dashboards.Features.GetDashboard;

internal class GetDashboardErrors
{
    public static Error DashboardNotFound => Error.NotFound("DashboardNotFound", "Dashboard not found.");
}
