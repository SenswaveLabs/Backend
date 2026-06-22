using Senswave.Devices.Domain.ShareDevices.Enums;

namespace Senswave.Devices.Domain.ShareDevices.Extensions;

public static class DeviceSharingTypeExtensions
{
    public static DeviceSharingType ToDeviceSharingType(this string type) => type switch
    {
        "Action" => DeviceSharingType.Action,
        "Manage" => DeviceSharingType.Manage,
        "Display" => DeviceSharingType.Display,
        "" => DeviceSharingType.Empty,
        _ => DeviceSharingType.Invalid
    };

    public static string FromDeviceSharingType(this DeviceSharingType type) => type switch
    {
        DeviceSharingType.Action => "Action",
        DeviceSharingType.Manage => "Manage",
        DeviceSharingType.Display => "Display",
        DeviceSharingType.Empty => "",
        _ => "Invalid"
    };
}
