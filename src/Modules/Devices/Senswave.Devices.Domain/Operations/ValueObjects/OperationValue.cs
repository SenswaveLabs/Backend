using Senswave.Devices.Domain.Operations.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Senswave.Devices.Domain.Operations.ValueObjects;

public class OperationValue
{
    public Guid OperationId { get; set; }

    public Operation? Operation { get; set; }

    public JsonObject InternalValue { get; set; } = [];

    [NotMapped]
    public JsonNode Value
    {
        get => InternalValue["value"] ?? JsonValue.Create("");
        set => InternalValue["value"] = value;
    }

    public DateTime ProcessedAtUtc { get; set; } = DateTime.UtcNow;
}
