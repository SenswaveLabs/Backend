namespace Senswave.Automations.Application.Features.AutomationState;

public class AutomationStateValidator : AbstractValidator<AutomationStateCommand>
{
    public AutomationStateValidator()
    {
        RuleFor(x => x.AutomationId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.IsEnabled)
            .NotNull();
    }
}