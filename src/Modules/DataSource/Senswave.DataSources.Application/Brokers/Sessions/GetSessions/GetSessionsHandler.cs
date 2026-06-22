using Senswave.DataSources.Domain.Brokers.Sessions.Entities;
using Senswave.DataSources.Domain.Brokers.Sessions.Repositories;

namespace Senswave.DataSources.Application.Brokers.Sessions.GetSessions;

public class GetSessionsHandler(
    ISessionQueryRepository repository,
    ILogger<GetSessionsHandler> logger) : IQueryHandler<GetSessionsQuery, IEnumerable<Session>>
{
    public async Task<Result<IEnumerable<Session>>> Handle(GetSessionsQuery request, CancellationToken cancellationToken)
    {
        var skip = (request.Page - 1) * request.Size;

        var sessions = await repository.GetBrokerSessions(request.UserId,
            request.BrokerId,
            skip,
            request.Size,
            cancellationToken);

        if (sessions.Count == 0)
        {
            logger.LogError("[User: {userId}] [Broker: {brokerId}] No sessions found on page {page} with size {size}.",
                request.UserId, request.BrokerId, request.Page, request.Size);
            return Result<IEnumerable<Session>>.Failure(GetSessionsErrors.SessionsNotFound);
        }

        logger.LogInformation("[User: {userId}] [Broker: {brokerId}] Found {count} sessions on page {page} with size {size}.",
            request.UserId, request.BrokerId, sessions.Count, request.Page, request.Size);
        return Result<IEnumerable<Session>>.Success(sessions);
    }
}
