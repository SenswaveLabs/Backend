namespace Senswave.Devices.Application.Dashboards.Features.UpdateDashboard;

public class UpdateDashboardCommand : ICommand
{
    public Guid UserId { get; set; } = Guid.Empty;
    public Guid DashboardId { get; set; } = Guid.Empty;

    public string Name { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;
}