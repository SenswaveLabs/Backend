namespace Senswave.Automations.Application.Features.PutResultToAutomation;

public class PutResultValidator : AbstractValidator<PutResultCommand>
{
    public PutResultValidator()
    {
        RuleFor(x => x.AutomationId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Result)
            .NotEmpty();
        RuleFor(x => x.Result!.OperationId)
            .NotEmpty();
        RuleFor(x => x.Result!.ValueToSend)
            .NotEmpty();
    }
}