namespace Senswave.Devices.Application.Dashboards.Features.DeleteDashboard;

public class DeleteDashboardCommand : ICommand
{
    public Guid UserId { get; set; } = Guid.Empty;
    public Guid DashboardId { get; set; } = Guid.Empty;
}