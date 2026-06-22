namespace Senswave.Devices.Domain.Operations.Types.Characteristics.Range;

internal class RangeCharacteristicErrors
{
    public static Error InvalidCharacteristicRange = Error.Validation("InvalidCharacteristicRange", "The range of the operation is not set or is invalid.");

    public static Error InvalidCharacteristicStepType = Error.Validation("InvalidCharacteristicStepType", "The step type is not supported for this operation.");

    public static Error CharacteristicStepIsTooSmall = Error.Validation("CharacteristicStepIsTooSmall", "Value of step is too small");

    public static Error CharacteristicStepIsTooBig = Error.Validation("CharacteristicStepIsTooBig", "Value of step is too big.");
}
