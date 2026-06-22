using Senswave.Abstractions.Entities;
using Senswave.Automations.Domain.Enums;
using Senswave.Infrastructure.Validation;

namespace Senswave.Automations.Application.Features.CreateAutomation;

public class CreateAutomationValidator : AbstractValidator<CreateAutomationCommand>
{
    public CreateAutomationValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .StandardCharacterSetWithSpace()
            .MinimumLength(AllowedLengths.Names.MinLength)
            .MaximumLength(AllowedLengths.Names.MaxLength);

        RuleFor(x => x.Icon)
            .StandardCharacterSet()
            .MaximumLength(AllowedLengths.Icons.MaxLength);

        RuleFor(x => x.HomeId)
            .NotEmpty();

        RuleFor(x => x.ConditionConnector)
            .NotEqual(AutomationConditionConnector.Invalid)
            .NotEqual(AutomationConditionConnector.Empty);

        RuleFor(x => x.Conditions.Count)
            .GreaterThan(0)
            .LessThanOrEqualTo(7);

        RuleFor(x => x.Results.Count)
            .GreaterThan(0)
            .LessThanOrEqualTo(7);

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