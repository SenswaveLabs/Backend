namespace Senswave.Devices.Application.Devices.Features.DisplayDevices;

public class DisplayDevicesValidator : AbstractValidator<DisplayDevicesQuery>
{
    public DisplayDevicesValidator()
    {
        RuleFor(x => x.HomeReferenceId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.Size)
            .GreaterThanOrEqualTo(1);
    }
}
