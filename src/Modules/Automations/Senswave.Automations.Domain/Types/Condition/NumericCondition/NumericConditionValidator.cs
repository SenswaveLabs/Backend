using FluentValidation;

namespace Senswave.Automations.Domain.Types.Condition.NumericCondition;

public class NumericConditionValidator : AbstractValidator<NumericCondition>
{
    public NumericConditionValidator()
    {
        RuleFor(x => new NumericCondition
        {
            Min = x.Min,
            Max = x.Max,
            OperationId = x.OperationId
        }).SetValidator(new BaseValidator());


        // This rule is derived from: if (x.Max and x.Min both exists) => (x.Min < x.Max)
        RuleFor(x => x)
            .Must(x => !x.Max.HasValue || !x.Min.HasValue || x.Min.Value < x.Max.Value);

        // Can not set that there is no both min and max value
        RuleFor(x => x)
            .Must(x => x.Max.HasValue || x.Min.HasValue);
    }
}