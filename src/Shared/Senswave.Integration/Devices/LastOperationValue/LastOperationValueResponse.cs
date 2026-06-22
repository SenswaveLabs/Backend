using Senswave.Integration.Shared;
using System.Text.Json.Nodes;

namespace Senswave.Integration.Devices.LastOperationValue;

public record LastOperationValueResponse : BaseInternalResponse
{
    public JsonNode LastValue { get; set; } = JsonValue.Create("");
}