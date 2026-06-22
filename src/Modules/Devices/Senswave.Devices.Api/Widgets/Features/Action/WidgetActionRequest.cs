using System.Text.Json.Nodes;

namespace Senswave.Devices.Api.Widgets.Features.Action;

public class WidgetActionRequest
{
    public JsonValue Value { get; set; } = JsonValue.Create("");
}
