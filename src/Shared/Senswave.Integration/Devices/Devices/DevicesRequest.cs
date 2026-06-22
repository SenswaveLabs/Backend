namespace Senswave.Integration.Devices.Devices;

public record DevicesRequest
{
    public Guid HomeId { get; set; } = Guid.Empty;
    public Guid UserId { get; set; } = Guid.Empty;
}
