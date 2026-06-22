namespace Senswave.DataSources.Api.Brokers.Brokers.GetBroker;

public class GetBrokerResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;

    public string Url { get; init; } = string.Empty;
    public string ProtocolVersion { get; init; } = string.Empty;
    public string ClientName { get; init; } = string.Empty;
    public int Port { get; init; }
    public bool UseTls { get; init; }

    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
