using System.Text.Json.Nodes;

namespace Senswave.TestInfrastructure.TestSetup.Models.Devices;

public class DeviceTileResponse
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("operationId")]
    public string? OperationId { get; set; }

    [JsonPropertyName("displayableOperationId")]
    public string? DisplayableOperationId { get; set; }

    [JsonPropertyName("configuration")]
    public JsonObject? Configuration { get; set; }
}
