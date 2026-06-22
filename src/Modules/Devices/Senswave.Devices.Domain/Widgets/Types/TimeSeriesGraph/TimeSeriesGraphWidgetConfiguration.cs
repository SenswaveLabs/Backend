using Senswave.Devices.Domain.Widgets.Types.Base;

namespace Senswave.Devices.Domain.Widgets.Types.TimeSeriesGraph;

public sealed class TimeSeriesGraphWidgetConfiguration : BaseWidgetConfiguration
{
    [JsonPropertyName("displayUnit")]
    public string DisplayUnit { get; set; } = string.Empty;

    [JsonPropertyName("initialNumberOfData")]
    public int InitialNumberOfData { get; set; } = 100;

    [JsonPropertyName("displayType")]
    public string DisplayTypeString { get; set; } = "lines";

    [JsonIgnore]
    public TimeSeriesDisplayType DisplayType => DisplayTypeString.FromDisplayType();
}
