using Senswave.Devices.Domain.Devices.Models;

namespace Senswave.Devices.Application.Devices.Features.DisplayDevice;

public class DisplayDeviceQuery : IQuery<DisplayDeviceModel>
{
    public Guid UserId { get; set; }
    public Guid DeviceId { get; set; }
}
