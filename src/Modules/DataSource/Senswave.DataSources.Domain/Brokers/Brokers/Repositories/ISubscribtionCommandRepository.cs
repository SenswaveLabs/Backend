using Senswave.DataSources.Domain.Brokers.Brokers.Entities;

namespace Senswave.DataSources.Domain.Brokers.Brokers.Repositories;

public interface ISubscribtionCommandRepository
{
    Task<Subscribtion?> GetSubscriptionWithBroker(Guid subscriptionId, CancellationToken cancellationToken);

    Task<Subscribtion?> GetSubscriptionByTopic(Guid dataSourceId, string topic, CancellationToken cancellationToken);

    Task<Result> CreateSubscribtion(Guid dataSourceId, string topic, CancellationToken cancellationToken);
    Task<Result> DeleteSubscribtion(Guid subscriptionId, CancellationToken cancellationToken);
    Task<Result> DeleteAllSubscribtions(Guid dataSourceId, CancellationToken cancellationToken);
}
