namespace Senswave.Integration.DataSource.BrokerConnection.Logging;

public record BaseLog
{
    public Guid BrokerId { get; set; } = Guid.Empty;
    public Guid SessionId { get; set; } = Guid.Empty;
}
