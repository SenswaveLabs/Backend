using Senswave.Integration.Shared;

namespace Senswave.Integration.DataSource.SubscribtionTopic;

public record SubscribtionTopicResponse : BaseInternalResponse
{
    public string Topic { get; set; } = string.Empty;
}