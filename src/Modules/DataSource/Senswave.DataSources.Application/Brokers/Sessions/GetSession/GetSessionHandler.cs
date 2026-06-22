using Senswave.DataSources.Domain.Brokers.Sessions.Entities;
using Senswave.DataSources.Domain.Brokers.Sessions.Repositories;

namespace Senswave.DataSources.Application.Brokers.Sessions.GetSession;

public class GetSessionHandler(
    ISessionQueryRepository repository,
    ILogger<GetSessionHandler> logger) : IQueryHandler<GetSessionQuery, Session>
{
    public async Task<Result<Session>> Handle(GetSessionQuery request, CancellationToken cancellationToken)
    {
        var session = await repository.GetBrokerSessionWithLogs(
            request.BrokerId,
            request.SessionId,
            request.UserId,
            cancellationToken);

        if (session is null)
        {
            logger.LogWarning("[User: {userId}] [Session: {sessionId}] Session not found for user.",
                request.UserId,
                request.SessionId);
            return Result<Session>.Failure(GetSessionErrors.SessionNotFound);
        }

        logger.LogInformation("[User: {userId}][Session: {sessionId}] Session found for user.",
            request.UserId,
            request.SessionId);

        return Result<Session>.Success(session);
    }
}
