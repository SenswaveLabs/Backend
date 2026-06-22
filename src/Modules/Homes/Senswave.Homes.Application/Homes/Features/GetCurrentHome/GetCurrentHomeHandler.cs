using Senswave.Homes.Domain.Homes.Entities;
using Senswave.Homes.Domain.Homes.Extensions;
using Senswave.Homes.Domain.Homes.Features.GetCurrentHome;

namespace Senswave.Homes.Application.Homes.Features.GetCurrentHome;

public class GetCurrentHomeHandler(
    IGetCurrentHomeRepository repository,
    ILogger<GetCurrentHomeHandler> logger) : IQueryHandler<GetCurrentHomeQuery, Home>
{
    public async Task<Result<Home>> Handle(GetCurrentHomeQuery request, CancellationToken cancellationToken)
    {
        var homes = await repository.GetHomes(request.UserId, cancellationToken);

        if (homes.Count == 0)
        {
            logger.LogWarning("[User: {UserId}] No homes found.", request.UserId);
            return Result<Home>.Failure(GetCurrentHomeErrors.HomeNotFound);
        }

        var currentHome = GetCurrentHome(homes, request.UserId, request.Longitude, request.Latitude);

        logger.LogInformation("[User: {UserId}] Retrieved current home {HomeId}.", request.UserId, currentHome.Id);
        return Result<Home>.Success(currentHome);
    }

    private Home GetCurrentHome(List<Home> homes, Guid userId, double? longitude, double? latitude)
    {
        try
        {
            if (longitude.HasValue && latitude.HasValue)
            {
                return homes
                    .OrderBy(x => x.Location != null ? x.Location.CalculateDistanceTo(latitude.Value, longitude.Value) : double.MaxValue)
                    .ThenBy(x => x.Location == null ? 1 : 0)
                    .ThenBy(x => x.OwnerId == userId ? 0 : 1)
                    .ThenBy(x => x.CreatedAtUtc)
                    .First();
            }

            return homes
                .OrderBy(x => x.OwnerId == userId ? 0 : 1)
                .ThenBy(x => x.CreatedAtUtc)
                .First();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while getting current home that requires investigation.");
            return homes.First();
        }
    }
}