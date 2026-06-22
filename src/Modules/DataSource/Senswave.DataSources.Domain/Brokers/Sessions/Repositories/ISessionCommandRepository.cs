using Senswave.DataSources.Domain.Brokers.Sessions.Entities;

namespace Senswave.DataSources.Domain.Brokers.Sessions.Repositories;

public interface ISessionCommandRepository
{
    Task<Result> CreateSession(Session session, CancellationToken cancellationToken);
    Task<Result> FinishSessions(Guid brokerId, CancellationToken cancellationToken);
    Task<Result> FinishSession(Session session, CancellationToken cancellationToken);
    Task<Result> FinishSession(Guid sessionId, CancellationToken cancellationToken);
    Task<Result> CreateSessionLog(Guid sessionId, Log log, CancellationToken cancellationToken);
    Task<Result> CreateSessionLogForCurrentSession(Guid brokerId, Log log, CancellationToken cancellationToken);
}
