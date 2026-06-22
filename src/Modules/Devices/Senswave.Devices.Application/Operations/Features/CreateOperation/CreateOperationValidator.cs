using Senswave.Abstractions.Entities;
using Senswave.Devices.Domain.Operations.Enums;

namespace Senswave.Devices.Application.Operations.Features.CreateOperation;

public class CreateOperationValidator : AbstractValidator<CreateOperationCommand>
{
    public CreateOperationValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .Length(AllowedLengths.Names.MinLength, AllowedLengths.Names.MaxLength);

        // Verified in datasource module
        RuleFor(x => x.Topic)
            .NotEmpty();

        RuleFor(x => x.Type)
            .NotEqual(OperationType.Invalid)
            .NotEqual(OperationType.Empty);
    }
}
