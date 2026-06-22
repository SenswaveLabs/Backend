using Senswave.Devices.Domain.Devices.TileTypes.Base;

namespace Senswave.Devices.Domain.Devices.TileTypes.Display;

public class DisplayDeviceTileConfiguration : BaseDeviceTileConfiguration
{
    [JsonPropertyName("unit")]
    public string Unit { get; init; } = string.Empty;
}
