using Senswave.Devices.Domain.ShareDevices.Enums;

namespace Senswave.Devices.Application.ShareDevices.Features.SetDeviceSharing;

public class SetDeviceSharingsValidator : AbstractValidator<SetDeviceSharingsCommand>
{
    public SetDeviceSharingsValidator()
    {
        RuleFor(x => x.FriendEmail)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.SharingType)
            .NotEqual(DeviceSharingType.Invalid)
            .NotEqual(DeviceSharingType.Empty);
    }
}