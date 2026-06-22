namespace Senswave.Integration.Devices.ActionAccessToOperations;

public record ActionAccessToOperationsRequest
{
    public Guid UserId { get; set; } = Guid.Empty;
    public HashSet<Guid> OperationIds { get; set; } = [];
}