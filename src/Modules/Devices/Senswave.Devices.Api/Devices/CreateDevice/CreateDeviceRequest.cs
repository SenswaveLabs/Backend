namespace Senswave.Devices.Api.Devices.CreateDevice;

public class CreateDeviceRequest
{
    public Guid HomeId { get; set; } = Guid.Empty;
    public Guid? RoomId { get; set; } = Guid.Empty;

    public string Name { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;
}