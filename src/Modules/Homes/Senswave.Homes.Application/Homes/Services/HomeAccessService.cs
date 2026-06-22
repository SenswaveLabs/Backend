using Senswave.Homes.Domain.Homes.Repositories;
using Senswave.Homes.Domain.Homes.Services;

namespace Senswave.Homes.Application.Homes.Services;

public class HomeAccessService(IHomeQueryRepository queryRepository,
    ILogger<HomeAccessService> logger) : IHomeAccessService
{
    #region Error    

    private readonly Error UserHasNoAccess = Error.Failure("UserHasNoAccessToHome", "User does not have access to this home.");

    #endregion

    public async Task<Result> IsOwner(Guid userId, Guid homeId, CancellationToken cancellationToken)
    {
        var isOwner = await queryRepository.IsHomeOwner(userId, homeId, cancellationToken);

        if (isOwner)
        {
            logger.LogInformation("[Home: {HomeId}] User {UserId} is the owner.", homeId, userId);
            return Result.Success();
        }

        logger.LogWarning("[Home: {HomeId}] User {UserId} is not the owner.", homeId, userId);
        return Result.Failure(UserHasNoAccess);
    }

    public async Task<Result> CanDisplay(Guid userId, Guid homeId, CancellationToken cancellationToken)
    {
        var isOwner = await queryRepository.IsHomeOwner(userId, homeId, cancellationToken);

        if (isOwner)
        {
            logger.LogInformation("[Home: {HomeId}] User {UserId} has display access as owner.", homeId, userId);
            return Result.Success();
        }

        var hasAccess = await queryRepository.HasDisplayAccessToHome(userId, homeId, cancellationToken);

        if (!hasAccess)
        {
            logger.LogWarning("[Home: {HomeId}] User {UserId} has no display access.", homeId, userId);
            return Result.Failure(UserHasNoAccess);
        }

        logger.LogInformation("[Home: {HomeId}] User {UserId} has display access.", homeId, userId);
        return Result.Success();
    }

    public async Task<Result> CanManage(Guid userId, Guid homeId, CancellationToken cancellationToken)
    {
        var isOwner = await queryRepository.IsHomeOwner(userId, homeId, cancellationToken);

        if (isOwner)
        {
            logger.LogInformation("[Home: {HomeId}] User {UserId} has manage access as owner.", homeId, userId);
            return Result.Success();
        }

        var hasAccess = await queryRepository.HasManageAccessToHome(userId, homeId, cancellationToken);

        if (!hasAccess)
        {
            logger.LogWarning("[Home: {HomeId}] User {UserId} has no manage access.", homeId, userId);
            return Result.Failure(UserHasNoAccess);
        }

        logger.LogInformation("[Home: {HomeId}] User {UserId} has manage access.", homeId, userId);
        return Result.Success();
    }
}
