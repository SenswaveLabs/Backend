using Senswave.Devices.Domain.Devices.Entities;

namespace Senswave.Devices.Application.Devices.Features.GetDevice;

public class GetDeviceQuery : IQuery<Device>
{
    public Guid DeviceId { get; set; } = Guid.Empty;

    public Guid UserId { get; set; } = Guid.Empty;
}