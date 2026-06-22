namespace Senswave.Devices.Application.ShareDevices.Features.GetDeviceSharings;

public class GetDeviceSharingsValidator : AbstractValidator<GetDeviceSharingsQuery>
{
    public GetDeviceSharingsValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.DeviceId)
            .NotEmpty();
    }
}