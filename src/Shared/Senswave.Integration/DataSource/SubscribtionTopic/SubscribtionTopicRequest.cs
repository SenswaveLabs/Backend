namespace Senswave.Integration.DataSource.SubscribtionTopic;
/*
 * Message
 * Ask DataSource about the string Topic, having subscriptionId
 */
public record SubscribtionTopicRequest
{
    public Guid SubscriptionId { get; set; } = Guid.Empty;
}