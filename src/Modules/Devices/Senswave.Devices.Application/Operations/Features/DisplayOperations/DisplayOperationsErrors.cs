namespace Senswave.Devices.Application.Operations.Features.DisplayOperations;

public static class DisplayOperationsErrors
{
    public static Error OperationsNotFound
        => Error.NotFound("OperationsNotFound", "Operations not found");

    public static Error DeviceNotFound =>
        Error.NotFound("DeviceNotFound", "Device with provided id not found in the database");
}