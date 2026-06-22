namespace Senswave.Devices.Application.Operations.Features.CreateOperation;

public class CreateOperationErrors
{
    public static Error OperationsLimitReached
        => Error.Conflict("OperationsLimitReached", "Operations limit reached");

    public static Error TopicUsedByAnotherDevice
        => Error.Conflict("TopicUsedByAnotherDevice", "Topic is already used by another device");

    public static Error FailedToCreateDataReference
        => Error.ServerError("FailedToCreateDataReference", "Failed to create data reference by topic");

    public static Error DeviceNotFound
        => Error.NotFound("DeviceNotFound", "Device not found");

    public static Error BrokerNotFound
        => Error.NotFound("BrokerNotFound", "Home does not have default broker");

    public static Error OperationCreationFailed
        => Error.ServerError("OperationCreationFailed", "Operation creation failed");

    public static Error NoAccess
        => Error.Failure("NoAccess", "No access to device");
}