using Senswave.TestInfrastructure.TestSetup.Models.Common;

namespace Senswave.TestInfrastructure.TestSetup.Models.Homes;

public class HomeListDto
{
    [JsonPropertyName("items")]
    public IdResponse[] Homes { get; set; } = [];
}
