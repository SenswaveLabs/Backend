namespace Senswave.TestInfrastructure.TestSetup.Models.DataSources;

public class SessionsListItemDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    [JsonPropertyName("updatedAtUtc")]
    public DateTime UpdatedAtUtc { get; set; }
    [JsonPropertyName("createdAtUtc")]
    public DateTime CreatedAtUtc { get; set; }
    [JsonPropertyName("finished")]
    public bool Finished { get; set; }
}
