using Microsoft.EntityFrameworkCore;
using Senswave.DataSources.Domain.Brokers.Brokers.Entities;
using Senswave.DataSources.Domain.Brokers.Brokers.Repositories;

namespace Senswave.DataSources.Infrastructure.Brokers.Brokers.Repositories;

public class SubscribtionQueryRepository(DataSourcesContext context) : ISubscribtionQueryRepository
{
    public Task<bool> BrokerExists(Guid brokerId, CancellationToken cancellationToken) => context.Brokers
        .Where(x => x.Id == brokerId)
        .AsNoTracking()
        .AnyAsync(cancellationToken);

    public Task<bool> BrokerExistsForOwner(Guid brokerId, Guid userId, CancellationToken cancellationToken) => context.Brokers
        .Where(x => x.Id == brokerId)
        .Where(x => x.OwnerId == userId)
        .AsNoTracking()
        .AnyAsync(cancellationToken);

    public Task<int> CountSubscribtions(Guid brokerId, CancellationToken cancellationToken) => context.Subscribtions
        .Where(x => x.BrokerId ==  brokerId)
        .AsNoTracking()
        .CountAsync(cancellationToken);

    public Task<Subscribtion?> GetSubscribtion(Guid brokerId, string topic, CancellationToken cancellationToken) => context.Subscribtions
        .Where(x => x.BrokerId == brokerId)
        .Where(x => x.Topic == topic)
        .AsNoTracking()
        .FirstOrDefaultAsync(cancellationToken);

    public Task<Subscribtion?> GetSubscription(Guid subscriptionId, CancellationToken cancellationToken) => context.Subscribtions
        .Where(x => x.Id == subscriptionId)
        .AsNoTracking()
        .FirstOrDefaultAsync(cancellationToken);

    public Task<List<Subscribtion>> GetSubscribtions(Guid userId, Guid brokerId, int skip, int size, CancellationToken cancellationToken) => context.Subscribtions
        .Where(x => x.BrokerId == brokerId)
        .Where(x => x.Broker.OwnerId == userId)
        .OrderBy(x => x.Topic)
        .Skip(skip)
        .Take(size)
        .AsNoTracking()
        .ToListAsync(cancellationToken);
}
