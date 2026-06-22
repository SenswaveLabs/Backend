namespace Senswave.Integration.DataSource.BrokerConnection.Restart;

public record RestartClientRequest
{
    public Guid BrokerId { get; set; }
}
