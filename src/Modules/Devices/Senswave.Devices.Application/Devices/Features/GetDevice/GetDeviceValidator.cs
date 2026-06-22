namespace Senswave.Devices.Application.Devices.Features.GetDevice;

public class GetDeviceValidator : AbstractValidator<GetDeviceQuery>
{
    public GetDeviceValidator()
    {
        RuleFor(d => d.UserId)
            .NotEmpty();

        RuleFor(d => d.DeviceId)
            .NotEmpty();
    }
}