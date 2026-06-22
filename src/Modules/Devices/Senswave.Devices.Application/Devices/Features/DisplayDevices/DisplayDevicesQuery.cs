using Senswave.Devices.Domain.Devices.Models;

namespace Senswave.Devices.Application.Devices.Features.DisplayDevices;

public class DisplayDevicesQuery : IQuery<List<DisplayDeviceModel>>
{
    public Guid UserId { get; set; }
    public Guid HomeReferenceId { get; set; }

    public int Page { get; set; } = int.MaxValue;
    public int Size { get; set; } = int.MaxValue;
}
