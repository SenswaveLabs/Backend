namespace Senswave.Devices.Domain.Devices.Models;

public class DisplayDeviceModel
{
    public Guid Id { get; set; }
    public Guid? RoomId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;

    public DisplayDeviceTileModel Tile { get; set; } = new();

    public DisplayPresenceModel Presence { get; set; } = new();

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}
