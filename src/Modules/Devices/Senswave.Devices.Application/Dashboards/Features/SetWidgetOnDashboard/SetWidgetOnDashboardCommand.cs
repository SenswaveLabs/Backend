namespace Senswave.Devices.Application.Dashboards.Features.SetWidgetOnDashboard;

public class SetWidgetOnDashboardCommand : ICommand
{
    public Guid UserId { get; set; }

    public Guid DashboardId { get; set; }

    public Guid WidgetId { get; set; }

    public int Row { get; set; }

    public int RowSpan { get; set; }

    public int Column { get; set; }

    public int ColumnSpan { get; set; }
}
