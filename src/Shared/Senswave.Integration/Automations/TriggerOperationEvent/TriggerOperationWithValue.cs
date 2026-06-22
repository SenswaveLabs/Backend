using System.Text.Json.Nodes;

namespace Senswave.Integration.Automations.TriggerOperationEvent;

public record TriggerOperationWithValue
{
    public Guid OperationId { get; set; } = Guid.Empty;

    public JsonValue Value { get; set; } = JsonValue.Create(string.Empty);
}