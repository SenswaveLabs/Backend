using Microsoft.EntityFrameworkCore;
using Senswave.DataSources.Domain.Brokers.Brokers.Entities;
using Senswave.DataSources.Domain.Brokers.Brokers.Repositories;

namespace Senswave.DataSources.Infrastructure.Brokers.Brokers.Repositories;

internal sealed class BrokerQueryRepository(DataSourcesContext context) : IBrokerQueryRepository
{
    public Task<Broker?> GetBroker(Guid userId, Guid brokerId, CancellationToken cancellationToken) => context.Brokers
        .Where(x => x.Id == brokerId)
        .Where(x => x.OwnerId == userId)
        .AsNoTracking()
        .FirstOrDefaultAsync(cancellationToken);

    public Task<Broker?> GetBroker(Guid brokerId, CancellationToken cancellationToken) => context.Brokers
        .Where(x => x.Id == brokerId)
        .AsNoTracking()
        .FirstOrDefaultAsync(cancellationToken);

    public Task<List<Broker>> GetBrokers(Guid userId, int skip, int size, CancellationToken cancellationToken) => context.Brokers
        .Where(b => b.OwnerId == userId)
        .Skip(skip)
        .Take(size)
        .AsNoTracking()
        .ToListAsync(cancellationToken);

    public Task<bool> BrokerNameUsedByUser(string name, Guid userId, CancellationToken cancellationToken) => context.Brokers
        .Where(x => x.Name == name)
        .Where(x => x.OwnerId == userId)
        .AsNoTracking()
        .AnyAsync(cancellationToken);

    public Task<bool> BrokerExists(string url, int port, CancellationToken cancellationToken) => context.Brokers
        .Where(x => x.BrokerInfo.Url == url)
        .Where(x => x.BrokerInfo.Port == port)
        .AsNoTracking()
        .AnyAsync(cancellationToken);

    public Task<bool> ClientNameIsUsedForBroker(string clientName, string url, int port, CancellationToken cancellationToken) => context.Brokers
        .Where(x => x.BrokerInfo.ClientName == clientName)
        .Where(x => x.BrokerInfo.Url == url)
        .Where(x => x.BrokerInfo.Port == port)
        .AsNoTracking()
        .AnyAsync(cancellationToken);

    public Task<int> CountBrokersForUser(Guid userId, CancellationToken cancellationToken) => context.Brokers
        .Where(x => x.OwnerId == userId)
        .AsNoTracking()
        .CountAsync(cancellationToken);

    public Task<int> CountGlobalBrokers(CancellationToken cancellationToken) => context.Brokers
        .CountAsync(cancellationToken);
}
