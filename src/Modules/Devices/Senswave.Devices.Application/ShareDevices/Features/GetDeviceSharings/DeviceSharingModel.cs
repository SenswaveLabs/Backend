using Senswave.Devices.Domain.ShareDevices.Enums;

namespace Senswave.Devices.Application.ShareDevices.Features.GetDeviceSharings;

public class DeviceSharingModel
{
    public Guid? SharingId { get; set; }

    public string FriendEmail { get; set; } = string.Empty;

    public DeviceSharingType SharingType { get; set; }
}