using Microsoft.EntityFrameworkCore;
using Senswave.DataSources.Domain.Brokers.Brokers.Entities;
using Senswave.DataSources.Domain.Brokers.Sessions.Entities;
using Senswave.DataSources.Domain.Brokers.Sessions.Repositories;

namespace Senswave.DataSources.Infrastructure.Brokers.Sessions.Repositories;

internal sealed class SessionQueryRepository(DataSourcesContext context) : ISessionQueryRepository
{
    public Task<Broker?> GetBroker(Guid userId, Guid brokerId, CancellationToken cancellationToken) => context.Brokers
        .Where(x => x.OwnerId == userId)
        .Where(x => x.Id == brokerId)
        .Include(x => x.Subscribtions)
        .AsNoTracking()
        .FirstOrDefaultAsync(cancellationToken);

    public Task<List<Session>> GetBrokerSessions(Guid userId, Guid brokerId, int skip, int size, CancellationToken cancellationToken) => context.Sessions
        .Where(x => x.Broker.Id == brokerId)
        .Where(x => x.Broker.OwnerId == userId)
        .OrderByDescending(x => x.CreatedAtUtc)
        .Skip(skip)
        .Take(size)
        .AsNoTracking()
        .ToListAsync(cancellationToken);

    public Task<Session?> GetLatestSessionForBroker(Guid brokerId, Guid userId, CancellationToken cancellationToken) => context.Sessions
        .Where(x => x.Broker.Id == brokerId)
        .Where(x => x.Broker.OwnerId == userId)
        .OrderBy(x => x.CreatedAtUtc)
        .AsNoTracking()
        .LastOrDefaultAsync(cancellationToken);

    public Task<Session?> GetBrokerSessionWithLogs(Guid brokerId, Guid sessionId, Guid userId, CancellationToken cancellationToken) => context.Sessions
        .Where(x => x.Broker.Id == brokerId)
        .Where(x => x.Broker.OwnerId == userId)
        .Where(x => x.Id == sessionId)
        .Include(x => x.Logs)
        .AsNoTracking()
        .FirstOrDefaultAsync(cancellationToken);
}
