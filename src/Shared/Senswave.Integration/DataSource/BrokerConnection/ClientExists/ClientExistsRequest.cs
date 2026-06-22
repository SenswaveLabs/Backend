namespace Senswave.Integration.DataSource.BrokerConnection.ClientExists;

public record ClientExistsRequest
{
    public Guid BrokerId { get; set; }
}
