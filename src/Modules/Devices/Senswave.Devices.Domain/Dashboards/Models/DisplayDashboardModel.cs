using Senswave.Devices.Domain.Dashboards.Enums;

namespace Senswave.Devices.Domain.Dashboards.Models;

public class DisplayDashboardModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DashboardType Type { get; set; }

    public JsonObject Configuration { get; set; } = [];
}
