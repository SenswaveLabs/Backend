namespace Senswave.Devices.Application.ShareDevices.Features.DeleteDeviceSharing;

public static class DeleteDeviceSharingErrors
{
    public static Error DeviceSharingNotFound => Error.NotFound("DeviceSharingNotFound",
        "DeviceSharing with provided id not found in the database");
}