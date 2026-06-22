using Senswave.DataSources.Domain.Brokers.Brokers.Entities;
using Senswave.DataSources.Domain.Brokers.Brokers.Repositories;

namespace Senswave.DataSources.Application.Brokers.Brokers.Features.GetBroker;

public class GetBrokerQueryHandler(
    IBrokerQueryRepository repository,
    ILogger<GetBrokerQueryHandler> logger) : IQueryHandler<GetBrokerQuery, Broker>
{
    public async Task<Result<Broker>> Handle(GetBrokerQuery request, CancellationToken cancellationToken)
    {
        var broker = await repository.GetBroker(request.UserId, request.BrokerId, cancellationToken);

        if (broker is null)
        {
            logger.LogWarning("[User: {userId}] Broker with id = {brokerId} not found.", request.UserId, request.BrokerId);
            return Result<Broker>.Failure([GetBrokerErrors.NotFound]);
        }

        logger.LogDebug("[User: {userId}] [Broker: {brokerId}] Broker found.", request.UserId, request.BrokerId);
        return Result<Broker>.Success(broker);
    }
}
