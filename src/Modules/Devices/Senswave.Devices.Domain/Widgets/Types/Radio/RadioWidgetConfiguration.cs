using Senswave.Devices.Domain.Widgets.Types.Base;
using Senswave.Devices.Domain.Widgets.Types.Radio.Model;

namespace Senswave.Devices.Domain.Widgets.Types.Radio;

public class RadioWidgetConfiguration : BaseWidgetConfiguration
{
    [JsonPropertyName("options")]
    public List<RadioOption> Options { get; set; } = [];
}
