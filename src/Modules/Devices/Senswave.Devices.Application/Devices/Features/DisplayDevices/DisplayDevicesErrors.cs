namespace Senswave.Devices.Application.Devices.Features.DisplayDevices;

internal static class DisplayDevicesErrors
{
    public static Error DevicesNotFound => Error.NotFound("DevicesNotFound", "No devices found.");
    public static Error NoAccess => Error.Failure("NoAccess", "No devices found.");
}
