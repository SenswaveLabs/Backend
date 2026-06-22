

namespace Senswave.Devices.Application.Devices.Errors;

internal class DeviceServiceErrors
{
    public static Error FailedToCreateOperationForDevicePresence = Error.ServerError("FailedToCreateOperationForDevicePresence", "Failed to create operation for device presence, please try again or try to reassign operation.");

    public static Error InvalidValueTypeForOpeation = Error.Validation("InvalidValueTypeForOpeation", "The value type provided for the operation is invalid, please verify the device tile configuration.");

    public static Error FailedToFindDevice = Error.NotFound("FailedToFindDevice", "Device not found as a user device.");

    public static Error DeviceIsNotPresent = Error.Conflict("DeviceIsNotPresent", "Device is not connected to the service currently.");

    public static Error UnsupportedDeviceTileType = Error.Failure("UnsupportedDeviceTileType", "Device tile is not allowed to send updates, please verify device tile configuration.");
}
