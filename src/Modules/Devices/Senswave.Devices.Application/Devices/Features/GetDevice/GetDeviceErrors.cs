namespace Senswave.Devices.Application.Devices.Features.GetDevice;

public static class GetDeviceErrors
{
    public static Error NotFound => Error.NotFound("DeviceNotFound", "Device not found.");
    public static Error NoAccess => Error.NotFound("NoAccess", "Access to this device is denied.");
}