using System.Text.Json.Nodes;

namespace Senswave.Automations.Api.Features.CreateAutomation;

public class ResultDto
{
    public Guid OperationId { get; set; } = Guid.Empty;

    public JsonValue ValueToSend { get; set; } = JsonValue.Create(string.Empty);
}