using System.Text.Json.Nodes;

namespace Senswave.Automations.Api.Features.CreateAutomation;

public class ConditionDto
{
    public Guid OperationId { get; set; } = Guid.Empty;

    // Can be Boolean, Numeric or Text
    public string ConditionType { get; set; } = string.Empty;

    public JsonObject ConditionConfiguration { get; set; } = [];
}