using Senswave.Devices.Domain.Devices.Entities;

namespace Senswave.Devices.Application.Devices.Features.CreateDevice;

public class CreateDeviceCommand : ICommand<Device>
{
    public Guid UserId { get; set; } = Guid.Empty;

    public Guid HomeId { get; set; } = Guid.Empty;
    public Guid? RoomId { get; set; } = Guid.Empty;

    public string Name { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;
}