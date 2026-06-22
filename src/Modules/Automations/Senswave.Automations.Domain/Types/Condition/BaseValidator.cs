using FluentValidation;

namespace Senswave.Automations.Domain.Types.Condition;

public class BaseValidator : AbstractValidator<BaseCondition>
{
    public BaseValidator()
    {
        RuleFor(x => x.OperationId)
            .NotEmpty();
    }
}