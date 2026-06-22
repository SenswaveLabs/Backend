namespace Senswave.Devices.Api.Devices.DisplayDevice;

public class DevicePresenceDto
{
    public string Type { get; set; } = string.Empty;
    public bool? Value { get; set; }
    public DateTime? LastSeenAtUtc { get; set; }
}
