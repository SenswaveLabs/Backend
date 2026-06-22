namespace Senswave.Devices.Api.Devices.DisplayDevice;

public class DisplayDeviceDto
{
    public Guid Id { get; set; }
    public Guid? RoomId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;

    public DeviceTileDto Tile { get; set; } = new();
    public DevicePresenceDto Presence { get; set; } = new();

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}
