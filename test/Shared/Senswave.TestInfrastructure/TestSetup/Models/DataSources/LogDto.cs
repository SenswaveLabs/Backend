namespace Senswave.TestInfrastructure.TestSetup.Models.DataSources;

public class LogDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    [JsonPropertyName("eventType")]
    public string EventType { get; set; } = string.Empty;
    [JsonPropertyName("data")]
    public string Data { get; set; } = string.Empty;
}
