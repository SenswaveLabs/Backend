using System.Text.Json.Nodes;

namespace Senswave.TestInfrastructure.TestSetup.Models.Automations;

public class PostResultItemRequest
{
    [JsonPropertyName("operationId")]
    public Guid OperationId { get; set; } = Guid.Empty;

    [JsonPropertyName("valueToSend")]
    public JsonValue ValueToSend { get; set; } = JsonValue.Create(string.Empty);
}