using Senswave.Devices.Domain.Operations.ValueObjects;

namespace Senswave.Devices.Domain.Operations.Models;

public class OperationAssembledModel
{
    public bool SendEvents { get; set; } = false;

    public string Payload { get; set; } = string.Empty;

    public OperationValue Value { get; set; } = new();
}
