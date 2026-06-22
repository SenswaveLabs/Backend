namespace Senswave.Devices.Application.Operations.Features.GetOperation;

public class GetOperationValidator : AbstractValidator<GetOperationQuery>
{
    public GetOperationValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.OperationId)
            .NotEmpty();
    }
}