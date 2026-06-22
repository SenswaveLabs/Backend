using Senswave.DataSources.Domain.Brokers.Brokers.Entities;
using Senswave.DataSources.Domain.Brokers.Sessions.Entities;

namespace Senswave.DataSources.Domain.Brokers.Sessions.Repositories;

public interface ISessionQueryRepository
{
    Task<Broker?> GetBroker(Guid userId, Guid brokerId, CancellationToken cancellationToken);

    Task<Session?> GetLatestSessionForBroker(Guid brokerId, Guid userId, CancellationToken cancellationToken);

    Task<Session?> GetBrokerSessionWithLogs(Guid brokerId, Guid sessionId, Guid userId, CancellationToken cancellationToken);

    Task<List<Session>> GetBrokerSessions(Guid userId, Guid brokerId, int skip, int size, CancellationToken cancellationToken);
}
