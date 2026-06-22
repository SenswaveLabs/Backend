namespace Senswave.TestInfrastructure.TestSetup.Models.DataSources;

public class SubscribtionsDto
{
    [JsonPropertyName("items")]
    public IEnumerable<SubscribtionItemDto> Items { get; set; } = [];
}
