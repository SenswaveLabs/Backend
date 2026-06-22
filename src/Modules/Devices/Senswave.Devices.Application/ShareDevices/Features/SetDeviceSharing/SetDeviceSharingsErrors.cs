namespace Senswave.Devices.Application.ShareDevices.Features.SetDeviceSharing;

public static class SetDeviceSharingsErrors
{
    public static Error FailedToGetHomeUsers => Error.Failure("FailedToGetHomeUsers", "Failed to retrieve users for this home.");
    public static Error DeviceNotFound => Error.NotFound("DeviceNotFound", "Device not found.");

    public static Error UserNotFound => Error.NotFound("UserNotFound", "User not found.");

    public static Error NoAccess => Error.Failure("NoAccess", "User does not have access to perform this action.");

    public static Error SharingCreationFailed => Error.ServerError("SharingCreationFailed", "Failed to create device sharing.");

    public static Error HomeNotShared => Error.Failure("HomeNotShared", "The home is not shared with the specified user.");

    public static Error SelfSharing => Error.Failure("SelfSharing", "Cannot share device with yourself.");
}