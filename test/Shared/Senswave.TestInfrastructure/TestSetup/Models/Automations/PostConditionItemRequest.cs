using System.Text.Json.Nodes;

namespace Senswave.TestInfrastructure.TestSetup.Models.Automations;

public class PostConditionItemRequest
{
    [JsonPropertyName("operationId")]
    public Guid OperationId { get; set; } = Guid.Empty;

    [JsonPropertyName("conditionType")]
    public string ConditionType { get; set; } = string.Empty;

    [JsonPropertyName("conditionConfiguration")]
    public JsonObject ConditionConfiguration { get; set; } = [];
}