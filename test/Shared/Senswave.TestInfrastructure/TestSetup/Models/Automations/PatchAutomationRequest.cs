namespace Senswave.TestInfrastructure.TestSetup.Models.Automations;

public class PatchAutomationRequest
{
    // Automation View
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = string.Empty;

    // Can be either "And" or "Or"
    [JsonPropertyName("conditionConnector")]
    public string ConditionConnector { get; set; } = string.Empty;

    // Conditions
    [JsonPropertyName("conditions")]
    public IList<PostConditionItemRequest> Conditions { get; set; } = [];

    // Results
    [JsonPropertyName("results")]
    public IList<PostResultItemRequest> Results { get; set; } = [];
}