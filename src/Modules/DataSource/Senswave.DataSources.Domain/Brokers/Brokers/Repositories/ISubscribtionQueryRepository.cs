
using Senswave.DataSources.Domain.Brokers.Brokers.Entities;

namespace Senswave.DataSources.Domain.Brokers.Brokers.Repositories;

public interface ISubscribtionQueryRepository
{
    Task<bool> BrokerExists(Guid brokerId, CancellationToken cancellationToken);
    Task<bool> BrokerExistsForOwner(Guid brokerId, Guid userId, CancellationToken cancellationToken);
    Task<int> CountSubscribtions(Guid brokerId, CancellationToken cancellationToken);
    Task<Subscribtion?> GetSubscribtion(Guid brokerId, string topic, CancellationToken cancellationToken);
    Task<Subscribtion?> GetSubscription(Guid subscriptionId, CancellationToken cancellationToken);
    Task<List<Subscribtion>> GetSubscribtions(Guid userId, Guid brokerId, int skip, int size, CancellationToken cancellationToken);
}
