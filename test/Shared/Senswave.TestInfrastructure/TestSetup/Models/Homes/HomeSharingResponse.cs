namespace Senswave.TestInfrastructure.TestSetup.Models.Homes;

public class HomeSharingResponse
{
    [JsonPropertyName("items")]
    public List<HomeSharingDto> Items { get; set; } = [];
}
