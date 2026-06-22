namespace Senswave.Integration.Devices.LastOperationValue;

public record LastOperationValueRequest
{
    public Guid OperationId { get; set; } = Guid.Empty;
}