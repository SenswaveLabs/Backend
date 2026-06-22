using Senswave.Devices.Domain.Devices.Enums;

namespace Senswave.Devices.Domain.Devices.Extensions;

public static class DeviceTileTypeExtensions
{
    public static DeviceTileType ToDeviceTileType(this string type) => type.ToLowerInvariant() switch
    {
        "display" => DeviceTileType.Display, 
        "switch" => DeviceTileType.Switch,
        "default" => DeviceTileType.Default,
        "" => DeviceTileType.Empty,
        _ => DeviceTileType.Invalid
    };

    public static string FromDeviceTileType(this DeviceTileType type) => type switch
    {
        DeviceTileType.Switch => "Switch",
        DeviceTileType.Display => "Display",
        DeviceTileType.Default => "Default",
        _ => "",
    };
}
