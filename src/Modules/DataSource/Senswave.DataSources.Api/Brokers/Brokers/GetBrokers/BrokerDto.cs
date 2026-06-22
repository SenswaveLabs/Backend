namespace Senswave.DataSources.Api.Brokers.Brokers.GetBrokers;

public record BrokerDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Server { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
