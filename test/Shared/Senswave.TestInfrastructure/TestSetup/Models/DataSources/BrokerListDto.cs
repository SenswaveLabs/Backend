using Senswave.TestInfrastructure.TestSetup.Models.Common;

namespace Senswave.TestInfrastructure.TestSetup.Models.DataSources;

public class BrokerListDto
{
    [JsonPropertyName("items")]
    public IEnumerable<IdResponse> Items { get; set; } = [];
}
