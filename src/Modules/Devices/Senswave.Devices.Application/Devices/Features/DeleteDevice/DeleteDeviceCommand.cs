namespace Senswave.Devices.Application.Devices.Features.DeleteDevice;

public class DeleteDeviceCommand : ICommand
{
    public Guid UserId { get; set; } = Guid.Empty;
    public Guid DeviceId { get; set; } = Guid.Empty;

}