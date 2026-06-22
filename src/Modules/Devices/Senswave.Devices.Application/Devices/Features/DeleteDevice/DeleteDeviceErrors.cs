namespace Senswave.Devices.Application.Devices.Features.DeleteDevice;

public static class DeleteDeviceErrors
{
    public static Error DeviceNotFound
        => Error.NotFound("DeviceNotFound", "Device with provided Id not found in the database");

    public static Error DeviceIsFull =>
        Error.Conflict("DeviceIsFull", "Can not delete device with existing operation or dashboards");

    public static Error FailedToRemove = Error.ServerError("FailedToRemove", "Failed to remove device from database.");
}