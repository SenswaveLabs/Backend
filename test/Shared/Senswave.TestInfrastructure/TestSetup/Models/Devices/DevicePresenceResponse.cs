namespace Senswave.TestInfrastructure.TestSetup.Models.Devices;

public class DevicePresenceResponse
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("operationId")]
    public string? OperationId { get; set; }
}
