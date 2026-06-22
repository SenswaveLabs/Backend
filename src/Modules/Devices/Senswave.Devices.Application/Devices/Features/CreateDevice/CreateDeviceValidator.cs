using Senswave.Abstractions.Entities;
using Senswave.Infrastructure.Validation;

namespace Senswave.Devices.Application.Devices.Features.CreateDevice;

public class CreateDeviceValidator : AbstractValidator<CreateDeviceCommand>
{

    public CreateDeviceValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.HomeId)
            .NotEmpty();

        RuleFor(x => x.Icon)
            .StandardCharacterSet()
            .MaximumLength(AllowedLengths.Icons.MaxLength);

        RuleFor(x => x.Name)
            .NotEmpty()
            .StandardCharacterSetWithSpace()
            .Length(AllowedLengths.Names.MinLength, AllowedLengths.Names.MaxLength);
    }
}