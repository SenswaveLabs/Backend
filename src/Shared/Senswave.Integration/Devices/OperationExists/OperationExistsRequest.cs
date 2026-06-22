namespace Senswave.Integration.Devices.OperationExists;

public record OperationExistsRequest()
{
    public Guid OperationId { get; init; } = Guid.Empty;
}