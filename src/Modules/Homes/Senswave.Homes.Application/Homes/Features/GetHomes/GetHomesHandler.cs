using Senswave.Homes.Domain.Homes.Entities;
using Senswave.Homes.Domain.Homes.Repositories;
using Senswave.Homes.Domain.Sharings.Enums;
using Senswave.Homes.Domain.Sharings.Repositories;

namespace Senswave.Homes.Application.Homes.Features.GetHomes;

public class GetHomesHandler(
    IHomeQueryRepository repository,
    IHomeSharingQueryRepository sharingRepository,
    ILogger<GetHomesHandler> logger)
    : IQueryHandler<GetHomesQuery, IEnumerable<Home>>
{
    public async Task<Result<IEnumerable<Home>>> Handle(GetHomesQuery request, CancellationToken cancellationToken)
    {
        var page = (request.Page - 1) * request.Size;

        var homes = await repository
            .GetHomes(request.UserId, page, request.Size, cancellationToken);

        var homeSharings = await sharingRepository
            .GetAllHomeSharingToUser(request.UserId, HomeSharingType.Display, cancellationToken);

        var sharedHomes = homeSharings
            .Select(x => x.Home)
            .ToList();

        if (homes.Count == 0 && sharedHomes.Count == 0)
        {
            logger.LogWarning("[User: {UserId}] No homes found.", request.UserId);
            return Result<IEnumerable<Home>>.Failure(GetHomesErrors.HomesNotFound);
        }

        var allHomesToReturn = homes
            .Concat(sharedHomes)
            .DistinctBy(x => x.Id)
            .ToList();

        logger.LogInformation("[User: {UserId}] Retrieved {Count} homes.", request.UserId, allHomesToReturn.Count);
        return Result<IEnumerable<Home>>.Success(allHomesToReturn);
    }
}