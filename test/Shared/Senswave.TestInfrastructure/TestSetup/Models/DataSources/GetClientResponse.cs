namespace Senswave.TestInfrastructure.TestSetup.Models.DataSources;

public class GetClientResponse
{
    [JsonPropertyName("connectionStatus")]
    public string ConnectionStatus { get; set; } = string.Empty;
    [JsonPropertyName("latestSessionId")]
    public Guid LatestSessionId { get; set; }
}
