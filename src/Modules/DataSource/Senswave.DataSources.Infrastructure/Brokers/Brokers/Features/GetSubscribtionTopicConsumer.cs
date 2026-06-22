using Senswave.DataSources.Domain.Brokers.Brokers.Repositories;
using Senswave.Integration.DataSource.SubscribtionTopic;
using Senswave.Integration.Shared;

namespace Senswave.DataSources.Infrastructure.Brokers.Brokers.Features;

public class GetSubscribtionTopicConsumer(
    ISubscribtionQueryRepository repository,
    ILogger<GetSubscribtionTopicConsumer> logger) : IConsumer<SubscribtionTopicRequest>
{
    public async Task Consume(ConsumeContext<SubscribtionTopicRequest> context)
    {
        var subscription = await repository.GetSubscription(context.Message.SubscriptionId, context.CancellationToken);

        if (subscription is null)
        {
            logger.LogWarning("[Subscription: {subscriptionId}] Failed to find subscription.", context.Message.SubscriptionId);
            await context.RespondAsync<SubscribtionTopicResponse>(BaseInternalResponse.Failure());
            return;
        }

        var response = new SubscribtionTopicResponse
        {
            Topic = subscription.Topic,
            StatusCode = InternalRequestStatus.Success
        };

        logger.LogInformation("[Subscription: {subscriptionId}] Found subscription with topic: {topic}.", context.Message.SubscriptionId, response.Topic);
        await context.RespondAsync(response);
    }
}