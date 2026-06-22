namespace Senswave.Devices.Api.Devices.GetDevice;

public record DeviceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public Guid? RoomId { get; set; }

    public DeviceTileDto Tile { get; set; } = new DeviceTileDto();
    public DevicePresenceDto Presence { get; set; } = new DevicePresenceDto();
}
