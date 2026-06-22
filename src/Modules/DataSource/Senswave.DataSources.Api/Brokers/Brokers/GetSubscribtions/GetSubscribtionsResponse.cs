namespace Senswave.DataSources.Api.Brokers.Brokers.GetSubscribtions;

public record GetSubscribtionsResponse
{
    public IEnumerable<SubscribtionDto> Items { get; init; } = [];
}
