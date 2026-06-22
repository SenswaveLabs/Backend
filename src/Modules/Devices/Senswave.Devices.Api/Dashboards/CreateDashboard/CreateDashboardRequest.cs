using System.Text.Json.Nodes;

namespace Senswave.Devices.Api.Dashboards.CreateDashboard;

public class CreateDashboardRequest
{
    public Guid DeviceId { get; set; } = Guid.Empty;

    public string Name { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;

    public JsonObject Configuration { get; set; } = [];
}