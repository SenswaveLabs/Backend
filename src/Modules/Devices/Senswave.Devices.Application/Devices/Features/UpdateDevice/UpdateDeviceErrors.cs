namespace Senswave.Devices.Application.Devices.Features.UpdateDevice;

public class UpdateDeviceErrors
{
    public static Error FailedToUpdate => Error.Failure("FailedToUpdate", "Failed to update device.");

    public static Error HomeNotFound =>
        Error.NotFound("HomeNotFound", "Home with provided id not found in the database.");

    public static Error BrokerNotAssigned =>
        Error.Conflict("BrokerNotAssigned", "Home with provided id does not have any datasource.");

    public static Error DeviceNotFound =>
        Error.NotFound("DeviceNotFound", "Device with provided id not found in the database.");

    public static Error NameAlreadyUsed =>
        Error.Conflict("NameAlreadyUsed", "Provided name has been assigned to another device.");

    public static Error RoomNotFound =>
        Error.NotFound("RoomNotFound", "Room with provided id not found in the database.");

    public static Error TileOperationNotFound =>
        Error.NotFound("TileOperationNotFound", "Tile operation is not found for device.");

    public static Error InvalidTileOperationType =>
        Error.Validation("InvalidTileOperationType", "Operation for tile must be boolean operation.");

    public static Error InvalidDisplayTileOperationType =>
        Error.Validation("InvalidDisplayTileOperationType", "Operation for display tile must be a numeric operation (Number or Integer).");

    public static Error PresenceOperationNotFound =>
        Error.NotFound("PresenceOperationNotFound", "Presence operation is not found for device.");

    public static Error InvalidPresenceOperationType =>
        Error.Validation("InvalidPresenceOperationType", "Operation for presence must be boolean operation.");
}