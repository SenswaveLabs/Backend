namespace Senswave.TestInfrastructure.TestSetup.Models.Devices;

public class GetDeviceResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = string.Empty;

    [JsonPropertyName("tile")]
    public DeviceTileResponse Tile { get; set; } = new();

    [JsonPropertyName("presence")]
    public DevicePresenceResponse Presence { get; set; } = new();

    [JsonPropertyName("roomId")]
    public Guid? RoomId { get; set; }
}