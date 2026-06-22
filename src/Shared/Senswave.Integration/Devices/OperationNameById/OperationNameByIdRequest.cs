namespace Senswave.Integration.Devices.OperationNameById;

public record OperationNameByIdRequest()
{
    public HashSet<Guid> OperationIds { get; set; } = [];
}