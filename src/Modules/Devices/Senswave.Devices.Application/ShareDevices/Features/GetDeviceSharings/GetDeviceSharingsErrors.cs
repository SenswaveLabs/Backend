namespace Senswave.Devices.Application.ShareDevices.Features.GetDeviceSharings;

public static class GetDeviceSharingsErrors
{
    public static Error DeviceNotFound =>
        Error.NotFound("DeviceNotFound", "Device with provided id not found in the database");
}