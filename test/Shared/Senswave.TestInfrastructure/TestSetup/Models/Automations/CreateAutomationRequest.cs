namespace Senswave.TestInfrastructure.TestSetup.Models.Automations;

public class CreateAutomationRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = string.Empty;

    [JsonPropertyName("homeId")]
    public Guid HomeId { get; set; } = Guid.Empty;

    [JsonPropertyName("conditionConnector")]
    public string ConditionConnector { get; set; } = string.Empty;

    [JsonPropertyName("conditions")]
    public IList<PostConditionItemRequest> Conditions { get; set; } = [];

    [JsonPropertyName("results")]
    public IList<PostResultItemRequest> Results { get; set; } = [];
}