using Senswave.DataSources.Domain.Brokers.Brokers.Entities;
using Senswave.DataSources.Domain.Brokers.Brokers.Repositories;

namespace Senswave.DataSources.Application.Brokers.Brokers.Features.GetBrokers;

public class GetBrokersHandler(
    IBrokerQueryRepository repository,
    ILogger<GetBrokersHandler> logger) : IQueryHandler<GetBrokersQuery, IEnumerable<Broker>>
{
    public async Task<Result<IEnumerable<Broker>>> Handle(GetBrokersQuery request, CancellationToken cancellationToken)
    {
        var skip = (request.Page - 1) * request.Size;

        var brokers = await repository.GetBrokers(request.UserId, skip, request.Size, cancellationToken);

        if (brokers.Count == 0)
        {
            logger.LogWarning("[User: {userId}] No brokers found for page {page} with size {size}.", request.UserId, request.Page, request.Size);
            return Result<IEnumerable<Broker>>.Failure([GetBrokersErrors.BrokersNotFound]);
        }

        logger.LogInformation("[User: {userId}] Found {count} brokers for page {page} with size {size}.", request.UserId, brokers.Count, request.Page, request.Size);
        return Result<IEnumerable<Broker>>.Success(brokers);
    }
}
