using Senswave.Automations.Domain.Enums;

namespace Senswave.Automations.Application.Features.PutConditionToAutomation;

public class PutConditionValidator : AbstractValidator<PutConditionCommand>
{
    public PutConditionValidator()
    {
        RuleFor(x => x.AutomationId)
            .NotEmpty();
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.AutomationCondition)
            .NotEmpty();

        RuleFor(x => x.AutomationCondition!.ConditionConfiguration)
            .NotEmpty();

        RuleFor(x => x.AutomationCondition!.ConditionType)
            .NotEqual(AutomationConditionType.Empty)
            .NotEqual(AutomationConditionType.Invalid);

        RuleFor(x => x.AutomationCondition!.OperationId)
            .NotEmpty();
    }
}