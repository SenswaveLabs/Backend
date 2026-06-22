namespace Senswave.DataSources.Api.Brokers.Brokers.CreateSubscribtion;

public record CreateSubscribtionRequest
{
    public string Topic { get; set; } = string.Empty;
}
