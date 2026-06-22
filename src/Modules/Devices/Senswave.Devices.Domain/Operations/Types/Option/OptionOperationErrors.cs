namespace Senswave.Devices.Domain.Operations.Types.Option;

internal class OptionOperationErrors
{
    public static Error FailedToMatchOptionByValue = Error.Failure("FailedToMatchOptionByValue", "Failed to match option by value.");

    public static Error FailedToCreatePayload = Error.Failure("FailedToCreatePayload", "Failed to create payload to send.");
}
