namespace Senswave.Devices.Application.Devices.Features.CreateDevice;

public class CreateDeviceError
{
    public static Error DataSourceNotAssigned
        => Error.Failure("DataSourceNotAssigned", "Please assign data source to home before creating devices.");

    public static Error DeviceLimitReached
        => Error.Conflict("DeviceLimitReached", "Limit of devices per home reached");

    public static Error FailedToCreate
        => Error.Failure("FailedToCreate", "Failed to create device.");

    public static Error AccessDeniedToHome
        => Error.Failure("AccessDeniedToHome", "Access to this home is denied.");

    public static Error HomeNotFound
        => Error.Failure("HomeNotFound", "Home not found.");

    public static Error NameAlreadyUsed
        => Error.Conflict("NameAlreadyUsed", "Device with provided name has already exists in this home");
}