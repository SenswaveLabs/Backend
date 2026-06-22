using System.Text.Json.Nodes;

namespace Senswave.Devices.Api.Dashboards.DisplayDashboard;

internal class DisplayDashboardResponse
{
    public string Type { get; set; } = string.Empty;

    public JsonObject Configuration { get; set; } = [];
}
