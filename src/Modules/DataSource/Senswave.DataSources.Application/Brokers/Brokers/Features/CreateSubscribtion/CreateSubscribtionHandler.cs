using Senswave.Abstractions.Messaging.Interfaces;
using Senswave.DataSources.Domain.Brokers.Brokers.Options;
using Senswave.DataSources.Domain.Brokers.Brokers.Repositories;
using Senswave.Integration.DataSource.BrokerConnection.Notifications;

namespace Senswave.DataSources.Application.Brokers.Brokers.Features.CreateSubscribtion;

internal sealed class CreateSubscribtionHandler(
    IMessageBus messageBus,
    ISubscribtionCommandRepository commandRepository,
    ISubscribtionQueryRepository queryRepository,
    IOptionsSnapshot<BrokerOptions> options,
    ILogger<CreateSubscribtionHandler> logger) : ICommandHandler<CreateSubscribtionCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateSubscribtionCommand request, CancellationToken cancellationToken)
    {
        var brokerExists = await queryRepository
            .BrokerExists(request.BrokerId, cancellationToken);

        if (!brokerExists)
        {
            logger.LogError("[Broker: {brokerId}] Failed to find broker for subscription creation.",
                request.BrokerId);

            return Result<Guid>.Failure(CreateSubscribtionErrors.BrokerNotFound);
        }

        var existingSubscribtion = await queryRepository
            .GetSubscribtion(request.BrokerId, request.Topic, cancellationToken);

        if (existingSubscribtion is not null)
        {
            logger.LogError("[Broker: {brokerId}] Subscription already exists for topic: {topic}.",
                request.BrokerId, request.Topic);

            if (request.FailWhenTopicAlreadyExists)
                return Result<Guid>.Failure(CreateSubscribtionErrors.SubscribtionAlreadyExists);

            return Result<Guid>.Success(existingSubscribtion.Id);
        }

        var currentCount = await queryRepository.CountSubscribtions(request.BrokerId, cancellationToken);

        if (options.Value.Limits.BrokerSubscribtions <= currentCount)
        {
            logger.LogWarning("[Broker: {brokerId}] Maximum number of subscriptions reached: {maxSubscriptions}.",
                request.BrokerId, options.Value.Limits.BrokerSubscribtions);

            return Result<Guid>.Failure(CreateSubscribtionErrors.MaximalNumberOfSubscribtions);
        }

        await commandRepository.CreateSubscribtion(request.BrokerId, request.Topic, cancellationToken);

        var createdSubscribtion = await queryRepository
            .GetSubscribtion(request.BrokerId, request.Topic, cancellationToken);

        if (createdSubscribtion is null)
        {
            logger.LogError("[Broker: {brokerId}] Failed to create subscription for topic: {topic}.",
                request.BrokerId, request.Topic);

            return Result<Guid>.Failure(CreateSubscribtionErrors.FailedToCreateSubscribtion);
        }

        var message = new SubscribeNotification
        {
            BrokerId = request.BrokerId,
            Topic = request.Topic
        };

        await messageBus.Publish(message, cancellationToken);

        logger.LogInformation("[Broker: {brokerId}] Created subscription for topic: {topic}.",
            request.BrokerId, request.Topic);

        return Result<Guid>.Success(createdSubscribtion.Id);
    }
}
