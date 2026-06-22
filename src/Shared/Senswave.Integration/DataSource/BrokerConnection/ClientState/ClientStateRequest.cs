namespace Senswave.Integration.DataSource.BrokerConnection.ClientState;

public record ClientStateRequest
{
    public Guid BrokerId { get; set; }
}
