using System.Text.Json.Nodes;

namespace Senswave.Devices.Api.Devices.DisplayDevice;

public class DeviceTileDto
{
    public string Type { get; set; } = string.Empty;
    public JsonNode? Value { get; set; }
    public JsonObject? Configuration { get; set; }
}
