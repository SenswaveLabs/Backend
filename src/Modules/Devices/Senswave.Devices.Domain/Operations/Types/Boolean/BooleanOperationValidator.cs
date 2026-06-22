using FluentValidation;
using Senswave.Devices.Domain.Operations.Types.Base;
using System.Text.Json;

namespace Senswave.Devices.Domain.Operations.Types.Boolean;

public class BooleanOperationValidator : BaseOperationValidator<BooleanOperation>
{
    public BooleanOperationValidator() : base()
    {
        RuleFor(x => x.Configuration.TrueValue)
            .Must(v => IsValidMappingValue(v!))
            .When(x => x.Configuration.TrueValue != null)
            .WithMessage("TrueValue must be a primitive JSON value (string 1-64 chars, number, or boolean).")
            .WithErrorCode("InvalidValueTypeForBooleanMapping");

        RuleFor(x => x.Configuration.FalseValue)
            .Must(v => IsValidMappingValue(v!))
            .When(x => x.Configuration.FalseValue != null)
            .WithMessage("FalseValue must be a primitive JSON value (string 1-64 chars, number, or boolean).")
            .WithErrorCode("InvalidValueTypeForBooleanMapping");

        RuleFor(x => x.Configuration)
            .Must(c => c.TrueValue!.ToJsonString() != c.FalseValue!.ToJsonString())
            .When(x => x.Configuration.TrueValue != null && x.Configuration.FalseValue != null)
            .WithMessage("TrueValue and FalseValue must be different.")
            .WithErrorCode("TrueAndFalseValueMustBeDifferent");

        RuleFor(x => x.Configuration)
            .Must(c => c.TrueValue != null && c.FalseValue != null)
            .When(x => x.Configuration.TrueValue != null || x.Configuration.FalseValue != null)
            .WithMessage("Both TrueValue and FalseValue must be provided if either is set.")
            .WithErrorCode("BothTrueAndFalseValueMustBeProvided");
    }

    private static bool IsValidMappingValue(JsonValue v)
    {
        return v.GetValueKind() switch
        {
            JsonValueKind.String => v.GetValue<string>().Length is >= 1 and <= 64,
            JsonValueKind.Number or JsonValueKind.True or JsonValueKind.False => true,
            _ => false
        };
    }
}
