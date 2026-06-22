using Refit;
using Senswave.Devices.Api.Dashboards.CreateDashboard;
using Senswave.Devices.Api.Dashboards.SetWidgetOnDashboard;

namespace Senswave.Presentation.Seed.Devices.Clients;

public interface IDashboardClient
{
    [Post("/v1/devices/dashboards")]
    Task<DashboardCreatedResponse> CreateDashboard([Authorize(scheme: "Bearer")] string token, CreateDashboardRequest home);

    [Put("/v1/devices/dashboards/{dashboardId}/widgets")]
    Task SetWidgetOnDashboard([Authorize(scheme: "Bearer")] string token, Guid dashboardId, SetWidgetOnDashboardRequest request);
}
