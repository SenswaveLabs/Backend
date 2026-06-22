namespace Senswave.Devices.Application.ShareDevices.Features.DeleteDeviceSharing;

public class DeleteDeviceSharingValidator : AbstractValidator<DeleteDeviceSharingCommand>
{
    public DeleteDeviceSharingValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.DeviceSharingId)
            .NotEmpty();
    }
}