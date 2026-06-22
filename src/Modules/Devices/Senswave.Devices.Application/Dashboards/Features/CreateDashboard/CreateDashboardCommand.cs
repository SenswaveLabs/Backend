using Senswave.Devices.Domain.Dashboards.Entities;
using Senswave.Devices.Domain.Dashboards.Enums;
using System.Text.Json.Nodes;

namespace Senswave.Devices.Application.Dashboards.Features.CreateDashboard;

public class CreateDashboardCommand : ICommand<Dashboard>
{
    public Guid UserId { get; set; } = Guid.Empty;

    public Guid DeviceId { get; set; } = Guid.Empty;

    public string Name { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;

    public DashboardType Type { get; set; } = DashboardType.Grid;

    public JsonObject Configuration { get; set; } = [];
}