namespace Senswave.Automations.Application.Features.DisplayAutomations;

public class DisplayAutomationsValidator : AbstractValidator<DisplayAutomationsQuery>
{
    public DisplayAutomationsValidator()
    {
        RuleFor(x => x.HomeId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}