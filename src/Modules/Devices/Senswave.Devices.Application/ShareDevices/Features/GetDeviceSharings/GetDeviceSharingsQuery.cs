namespace Senswave.Devices.Application.ShareDevices.Features.GetDeviceSharings;

public class GetDeviceSharingsQuery : IQuery<List<DeviceSharingModel>>
{
    public Guid UserId { get; set; } = Guid.Empty;

    public Guid DeviceId { get; set; } = Guid.Empty;
}