namespace Senswave.Automations.Application.Features.GetAutomation;

public class GetAutomationValidator : AbstractValidator<GetAutomationQuery>
{
    public GetAutomationValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.AutomationId)
            .NotEmpty();
    }
}