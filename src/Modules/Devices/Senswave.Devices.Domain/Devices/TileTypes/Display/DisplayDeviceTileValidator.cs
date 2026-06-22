using FluentValidation;

namespace Senswave.Devices.Domain.Devices.TileTypes.Display;

public class DisplayDeviceTileValidator : AbstractValidator<DisplayDeviceTile>
{
    public DisplayDeviceTileValidator()
    {
        RuleFor(x => x.Configuration.Unit)
            .MaximumLength(5)
            .WithMessage("Unit must be at most 5 characters.")
            .WithErrorCode("UnitTooLong");
    }
}
