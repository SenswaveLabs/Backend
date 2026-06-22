namespace Senswave.Devices.Application.Operations.Features.DeleteOperation;

public class DeleteOperationValidator : AbstractValidator<DeleteOperationCommand>
{
    public DeleteOperationValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
        RuleFor(x => x.OperationId)
            .NotEmpty();
    }
}