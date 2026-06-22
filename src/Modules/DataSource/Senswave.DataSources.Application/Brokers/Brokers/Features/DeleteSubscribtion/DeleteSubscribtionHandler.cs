using Senswave.Abstractions.Messaging.Interfaces;
using Senswave.DataSources.Domain.Brokers.Brokers.Repositories;
using Senswave.Integration.DataSource.BrokerConnection.Notifications;
using Senswave.Integration.Devices.SubscriptionUsage;

namespace Senswave.DataSources.Application.Brokers.Brokers.Features.DeleteSubscribtion;

internal sealed class DeleteSubscribtionHandler(
    IMessageBus messageBus,
    ISubscribtionCommandRepository commandRepository,
    ISubscribtionQueryRepository queryRepository,
    IRequestClient<SubscriptionUsageRequest> subscriptionUsageClient,
    ILogger<DeleteSubscribtionHandler> logger) : ICommandHandler<DeleteSubscribtionCommand>
{
    public async Task<Result> Handle(DeleteSubscribtionCommand request, CancellationToken cancellationToken)
    {
        var brokerExists = await queryRepository
            .BrokerExistsForOwner(request.BrokerId, request.UserId, cancellationToken);

        if (!brokerExists)
        {
            logger.LogWarning("[User: {userId}] [Broker: {brokerId}] Broker not found for subscription deletion.",
                request.UserId, request.BrokerId);

            return Result.Failure(DeleteSubscribtionErrors.BrokerNotFound);
        }

        var subscription = await queryRepository
            .GetSubscription(request.SubscriptionId, cancellationToken);

        if (subscription is null || subscription.BrokerId != request.BrokerId)
        {
            logger.LogWarning("[Broker: {brokerId}] [Subscription: {subscriptionId}] Subscription not found.",
                request.BrokerId, request.SubscriptionId);

            return Result.Failure(DeleteSubscribtionErrors.SubscribtionNotFound);
        }

        var usageResponse = await subscriptionUsageClient
            .GetResponse<SubscriptionUsageResponse>(
                new SubscriptionUsageRequest { SubscriptionId = request.SubscriptionId },
                cancellationToken);

        if (usageResponse.Message.OperationsCount > 0)
        {
            logger.LogWarning(
                "[Broker: {brokerId}] [Subscription: {subscriptionId}] Subscription is used by {count} operation(s).",
                request.BrokerId, request.SubscriptionId, usageResponse.Message.OperationsCount);

            return Result.Failure(DeleteSubscribtionErrors.SubscribtionUsedByOperations);
        }

        var deleteResult = await commandRepository.DeleteSubscribtion(request.SubscriptionId, cancellationToken);

        if (deleteResult.IsFailure)
        {
            logger.LogError("[Broker: {brokerId}] [Subscription: {subscriptionId}] Failed to delete subscription.",
                request.BrokerId, request.SubscriptionId);

            return Result.Failure(DeleteSubscribtionErrors.FailedToDeleteSubscribtion);
        }

        var message = new UnsubscribeNotifications
        {
            BrokerId = request.BrokerId,
            Topic = subscription.Topic
        };

        await messageBus.Publish(message, cancellationToken);

        logger.LogInformation("[Broker: {brokerId}] [Subscription: {subscriptionId}] Subscription deleted for topic: {topic}.",
            request.BrokerId, request.SubscriptionId, subscription.Topic);

        return Result.Success();
    }
}
