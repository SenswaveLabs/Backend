namespace Senswave.TestInfrastructure.TestSetup.Models.DataSources;

public class SessionDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; } = Guid.Empty;

    [JsonPropertyName("logs")]
    public IEnumerable<LogDto> Logs { get; set; } = [];
}
