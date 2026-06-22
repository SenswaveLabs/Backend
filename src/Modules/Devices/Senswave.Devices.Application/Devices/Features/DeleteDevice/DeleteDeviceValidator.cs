namespace Senswave.Devices.Application.Devices.Features.DeleteDevice;

public class DeleteDeviceValidator : AbstractValidator<DeleteDeviceCommand>
{
    public DeleteDeviceValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
        RuleFor(x => x.DeviceId)
            .NotEmpty();
    }
}