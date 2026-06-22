namespace Senswave.Automations.Application.Features.DeleteCondition;

public class DeleteConditionValidator : AbstractValidator<DeleteConditionCommand>
{
    public DeleteConditionValidator()
    {
        RuleFor(x => x.ConditionId)
            .NotEmpty();
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}