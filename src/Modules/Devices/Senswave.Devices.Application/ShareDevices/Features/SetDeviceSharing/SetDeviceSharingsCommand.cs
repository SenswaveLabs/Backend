using Senswave.Devices.Domain.ShareDevices.Enums;

namespace Senswave.Devices.Application.ShareDevices.Features.SetDeviceSharing;

public class SetDeviceSharingsCommand : ICommand
{
    public Guid UserId { get; set; } = Guid.Empty;

    public string FriendEmail { get; set; } = string.Empty;
    public Guid DeviceId { get; set; } = Guid.Empty;
    public DeviceSharingType SharingType { get; set; }
}