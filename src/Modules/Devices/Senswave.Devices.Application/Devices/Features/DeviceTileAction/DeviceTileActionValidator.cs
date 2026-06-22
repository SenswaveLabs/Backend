namespace Senswave.Devices.Application.Devices.Features.DeviceTileAction;

public class DeviceTileActionValidator : AbstractValidator<DeviceTileActionCommand>
{
    public DeviceTileActionValidator()
    {
        RuleFor(x => x.DeviceId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Value)
            .Must(x => x?.ToString().Length <= 512)
            .When(x => x.Value is not null);
    }
}
