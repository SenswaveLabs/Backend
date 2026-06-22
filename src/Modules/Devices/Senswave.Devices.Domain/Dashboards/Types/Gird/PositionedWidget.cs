namespace Senswave.Devices.Domain.Dashboards.Types.Gird;

public class PositionedWidget
{
    [JsonPropertyName("widgetId")]
    public Guid WidgetId { get; set; }

    [JsonPropertyName("row")]
    public int Row { get; set; }

    [JsonPropertyName("rowSpan")]
    public int RowSpan { get; set; }

    [JsonPropertyName("column")]
    public int Column { get; set; }

    [JsonPropertyName("columnSpan")]
    public int ColumnSpan { get; set; }
}
