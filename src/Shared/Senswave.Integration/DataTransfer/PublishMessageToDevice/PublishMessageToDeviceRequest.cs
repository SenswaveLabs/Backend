namespace Senswave.Integration.DataTransfer.PublishMessageToDevice;

public record PublishMessageToDeviceRequest
{
    public Guid DataSourceReferenceId { get; init; }
    public string Payload { get; init; } = string.Empty;
}
