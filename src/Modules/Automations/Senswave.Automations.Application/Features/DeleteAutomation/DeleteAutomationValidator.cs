namespace Senswave.Automations.Application.Features.DeleteAutomation;

public class DeleteAutomationValidator : AbstractValidator<DeleteAutomationCommand>
{
    public DeleteAutomationValidator()
    {
        RuleFor(x => x.AutomationId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}