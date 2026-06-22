using FluentValidation;

namespace Senswave.Automations.Domain.Types.Condition.TextCondition;

public class TextConditionValidator : AbstractValidator<TextCondition>
{
    public TextConditionValidator()
    {
        RuleFor(x => new TextCondition
        {
            OperationId = x.OperationId,
            RequiredValue = x.RequiredValue
        }).SetValidator(new BaseValidator());

        RuleFor(x => x.RequiredValue)
            .NotEmpty();
    }
}