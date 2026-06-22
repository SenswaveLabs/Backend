using System.Text.Json.Nodes;

namespace Senswave.Devices.Api.Dashboards.GetDashboard;

public class GetDashboardResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public JsonObject Configuration { get; set; } = [];
}
