namespace Senswave.Devices.Api.Dashboards.SetWidgetOnDashboard;

public class SetWidgetOnDashboardRequest
{
    public Guid WidgetId { get; set; }

    public int Row { get; set; }

    public int RowSpan { get; set; }

    public int Column { get; set; }

    public int ColumnSpan { get; set; }
}
