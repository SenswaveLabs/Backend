namespace Senswave.Devices.Application.Dashboards.Features.DeleteDashboardWidget;

public class DeleteDashboardWidgetCommand : ICommand
{
    public Guid UserId { get; set; }

    public Guid DashboardId { get; set; }

    public Guid WidgetId { get; set; }
}
