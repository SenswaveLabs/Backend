namespace Senswave.Devices.Application.Devices.Features.DisplayDevice;

internal static class DisplayDeviceErrors
{
    public static Error NotFound = Error.Failure("DeviceNotFound", "Device not found.");
    public static Error NoAccess = Error.Failure("NoAccess", "Access to this device is denied.");
}
