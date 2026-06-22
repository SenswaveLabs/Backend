namespace Senswave.TestInfrastructure.TestSetup.Models.DataSources;

public class SessionsDto
{
    [JsonPropertyName("items")]
    public IEnumerable<SessionsListItemDto> Items { get; set; } = [];
}
