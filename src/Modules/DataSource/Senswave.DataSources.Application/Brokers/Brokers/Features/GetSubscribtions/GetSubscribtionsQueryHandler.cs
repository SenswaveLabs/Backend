using Senswave.DataSources.Domain.Brokers.Brokers.Entities;
using Senswave.DataSources.Domain.Brokers.Brokers.Repositories;

namespace Senswave.DataSources.Application.Brokers.Brokers.Features.GetSubscribtions;

public class GetSubscribtionsQueryHandler(
    ISubscribtionQueryRepository repository,
    ILogger<GetSubscribtionsQueryHandler> logger) : IQueryHandler<GetSubscribtionsQuery, IEnumerable<Subscribtion>>
{
    public async Task<Result<IEnumerable<Subscribtion>>> Handle(GetSubscribtionsQuery request, CancellationToken cancellationToken)
    {
        var brokerExists = await repository.BrokerExists(request.BrokerId, cancellationToken);

        if (!brokerExists)
        {
            logger.LogWarning("[User: {userId}] Broker with id = {brokerId} not found.", request.UserId, request.BrokerId);
            return Result<IEnumerable<Subscribtion>>.Failure([GetSubscribtionsErrors.BrokerNotFound]);
        }

        var skip = (request.Page - 1) * request.Size;

        var subscribtions = await repository.GetSubscribtions(request.UserId, request.BrokerId, skip, request.Size, cancellationToken);

        if (subscribtions.Count == 0)
        {
            logger.LogWarning("[User: {userId}] [Broker: {brokerId}] No subscriptions found on page {page} with size {size}.",
                request.UserId, request.BrokerId, request.Page, request.Size);
            return Result<IEnumerable<Subscribtion>>.Failure([GetSubscribtionsErrors.SubscribtionsNotFound]);
        }

        logger.LogInformation("[User: {userId}] [Broker: {brokerId}] Found {count} subscriptions on page {page} with size {size}.",
            request.UserId, request.BrokerId, subscribtions.Count, request.Page, request.Size);
        return Result<IEnumerable<Subscribtion>>.Success(subscribtions);
    }
}
