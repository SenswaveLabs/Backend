namespace Senswave.Devices.Application.ShareDevices.Features.DeleteDeviceSharing;

public class DeleteDeviceSharingCommand : ICommand
{
    public Guid DeviceSharingId { get; set; } = Guid.Empty;

    public Guid UserId { get; set; } = Guid.Empty;
}