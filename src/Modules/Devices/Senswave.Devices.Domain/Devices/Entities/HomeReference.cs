namespace Senswave.Devices.Domain.Devices.Entities;

public class HomeReference : Entity
{
    public Guid OwnerId { get; set; }

    public Guid HomeId { get; set; }

    public List<Device> Devices { get; set; } = [];
}
