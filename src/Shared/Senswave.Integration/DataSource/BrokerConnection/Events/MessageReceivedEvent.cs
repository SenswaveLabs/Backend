namespace Senswave.Integration.DataSource.BrokerConnection.Events;

public class MessageReceivedEvent
{
    public Guid BrokerId { get; init; }
    public Guid SessionId { get; init; }
    public string Topic { get; init; } = string.Empty;
    public string Payload { get; init; } = string.Empty;
}
