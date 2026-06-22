namespace Senswave.DataSources.Api.Brokers.Brokers.GetSubscribtions;

public record SubscribtionDto
{
    public Guid Id { get; init; }
    public string Topic { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
