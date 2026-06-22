using Senswave.Devices.Domain.Devices.Enums;

namespace Senswave.Devices.Domain.Devices.Models;

public class DisplayDeviceTileModel
{
    public DeviceTileType Type { get; set; }

    public JsonNode? Value { get; set; }

    public JsonObject? Configuration { get; set; }
}
