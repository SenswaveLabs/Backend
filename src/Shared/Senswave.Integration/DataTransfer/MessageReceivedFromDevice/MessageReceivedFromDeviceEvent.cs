namespace Senswave.Integration.DataTransfer.MessageReceivedFromDevice;

public record MessageReceivedFromDeviceEvent
{
    public Guid BrokerId { get; init; }
    public Guid SubscribtionId { get; init; }
    public string Payload { get; init; } = string.Empty;
}
