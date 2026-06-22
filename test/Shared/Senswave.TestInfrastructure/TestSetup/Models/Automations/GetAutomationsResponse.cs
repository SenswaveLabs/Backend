using Senswave.TestInfrastructure.TestSetup.Models.Common;

namespace Senswave.TestInfrastructure.TestSetup.Models.Automations;

public class GetAutomationsResponse
{
    [JsonPropertyName("items")]
    public IList<IdResponse> Items { get; set; } = [];
}