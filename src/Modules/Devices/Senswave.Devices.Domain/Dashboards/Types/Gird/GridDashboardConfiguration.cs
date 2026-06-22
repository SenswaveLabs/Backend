namespace Senswave.Devices.Domain.Dashboards.Types.Gird;

public class GridDashboardConfiguration
{
    [JsonPropertyName("rows")]
    public int Rows { get; set; } = 0;

    [JsonPropertyName("columns")]
    public int Columns { get; set; } = 0;

    [JsonPropertyName("positionedWidgets")]
    public List<PositionedWidget> PositionedWidgets { get; set; } = [];
}
