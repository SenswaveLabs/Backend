namespace Senswave.DataSources.BrokerConnection.Clients.MqttV3;

public record PublishedMessage
{
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
