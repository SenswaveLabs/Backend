using Senswave.Devices.Domain.Widgets.Types.Base;

namespace Senswave.Devices.Domain.Widgets.Types.Button;

public class ButtonWidgetConfiguration : BaseWidgetConfiguration
{
    [JsonPropertyName("value")]
    public JsonValue Value { get; set; } = JsonValue.Create("");
}
