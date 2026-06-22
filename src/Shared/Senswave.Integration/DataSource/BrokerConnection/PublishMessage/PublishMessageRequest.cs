namespace Senswave.Integration.DataSource.BrokerConnection.PublishMessage;

public record PublishMessageRequest
{
    public Guid BrokerId { get; set; }
    public string Topic { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
}
