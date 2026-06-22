using Senswave.Devices.Domain.Devices.Enums;

namespace Senswave.Devices.Domain.Devices.Extensions;

public static class DevicePresenceTypeExtensions
{
    public static DevicePresenceType ToDevicePresenceType(this string type) => type switch
    {
        "BooleanOperation" => DevicePresenceType.BooleanOperation,
        "Default" => DevicePresenceType.Default,
        "" => DevicePresenceType.Empty,
        _ => DevicePresenceType.Invalid
    };

    public static string FromDevicePresenceType(this DevicePresenceType type) => type switch
    {
        DevicePresenceType.BooleanOperation => "BooleanOperation",
        DevicePresenceType.Default => "Default",
        _ => "",
    };
}
