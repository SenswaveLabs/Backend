using System.Text.Json.Nodes;

namespace Senswave.Devices.Api.Devices.DeviceTileAction;

public class DeviceTileActionRequest
{
    public JsonValue Value { get; set; } = JsonValue.Create("");
}
