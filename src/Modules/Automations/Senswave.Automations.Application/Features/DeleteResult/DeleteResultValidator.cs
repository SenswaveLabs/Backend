namespace Senswave.Automations.Application.Features.DeleteResult;

public class DeleteResultValidator : AbstractValidator<DeleteResultCommand>
{
    public DeleteResultValidator()
    {
        RuleFor(command => command.UserId)
            .NotEmpty();

        RuleFor(command => command.ResultId)
            .NotEmpty();
    }
}