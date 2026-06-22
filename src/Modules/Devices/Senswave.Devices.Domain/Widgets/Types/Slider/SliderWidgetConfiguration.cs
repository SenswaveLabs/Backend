using Senswave.Devices.Domain.Widgets.Types.Base;
using Senswave.Devices.Domain.Widgets.Types.Utils;

namespace Senswave.Devices.Domain.Widgets.Types.Slider;

public class SliderWidgetConfiguration : BaseWidgetConfiguration
{
    [JsonPropertyName("step")]
    [JsonConverter(typeof(NumericTypesConverter))]
    public object? Step { get; set; }
}
