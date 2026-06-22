using Senswave.Devices.Domain.Widgets.Types.Base;

namespace Senswave.Devices.Domain.Widgets.Types.Display;

public sealed class DisplayWidgetConfiguration : BaseWidgetConfiguration
{
    [JsonPropertyName("unit")]
    public string Unit { get; set; } = string.Empty;
}
