using FluentValidation;

namespace Senswave.Automations.Domain.Types.Condition.BooleanCondition;

public class BooleanConditionValidator : AbstractValidator<BooleanCondition>
{
    public BooleanConditionValidator()
    {
        RuleFor(x => new BooleanCondition
        {
            OperationId = x.OperationId,
            IsOn = x.IsOn
        }).SetValidator(new BaseValidator());

        RuleFor(x => x.IsOn)
            .NotNull();
    }
}