using Senswave.Abstractions.Entities;
using Senswave.Automations.Domain.Enums;

namespace Senswave.Automations.Application.Features.UpdateAutomation;

public class UpdateAutomationValidator : AbstractValidator<UpdateAutomationCommand>
{
    public UpdateAutomationValidator()
    {
        RuleFor(x => x.AutomationId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .Empty()
            .When(x => string.IsNullOrWhiteSpace(x.Name))
            .Length(AllowedLengths.Names.MinLength, AllowedLengths.Names.MaxLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Name));

        RuleFor(x => x.Icon)
            .MaximumLength(AllowedLengths.Icons.MaxLength);

        RuleFor(x => x.ConditionConnector)
            .NotEqual(AutomationConditionConnector.Invalid);

        RuleForEach(x => x.Conditions)
            .ChildRules(condition =>
            {
                condition.RuleFor(x => x.OperationId)
                    .NotEmpty();
                condition.RuleFor(x => x.ConditionType)
                    .NotEqual(AutomationConditionType.Empty)
                    .NotEqual(AutomationConditionType.Invalid);
                condition.RuleFor(x => x.ConditionConfiguration)
                    .NotEmpty();
            });

        RuleForEach(x => x.Results)
            .ChildRules(result =>
            {
                result.RuleFor(x => x.OperationId)
                    .NotEmpty();
                result.RuleFor(x => x.ValueToSend)
                    .NotEmpty();
            });
    }
}