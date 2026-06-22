namespace Senswave.Devices.Application.Devices.Features.DeviceTileAction;

internal class DeviceTileActionErrors
{
    public static Error DeviceNotFound => Error.NotFound("DeviceNotFound", "Device not found.");
    public static Error NoAccess => Error.NotFound("NoAccess", "Access to this device is denied.");
    public static Error DeviceHasNoTile => Error.NotFound("DeviceHasNoTile", "Device does not have a configured tile.");
}
