using Senswave.Homes.Domain.Sharings.Repositories;

namespace Senswave.Homes.Application.Sharings.Features.LeaveSharing;

public class LeaveSharingHandler(
    IHomeSharingCommandRepository repository,
    ILogger<LeaveSharingHandler> logger)
    : ICommandHandler<LeaveSharingCommand>
{
    public async Task<Result> Handle(LeaveSharingCommand request, CancellationToken cancellationToken)
    {
        var homeSharing = await repository.GetHomeSharingForUser(request.HomeId, request.UserId, cancellationToken);

        if (homeSharing is null)
        {
            logger.LogWarning("[Home Sharing] Not found for user {userId} in home {homeId}", request.UserId, request.HomeId);
            return Result.Failure(LeaveSharingErrors.FailedToLeaveHome);
        }

        var result = await repository.DeleteSharing(homeSharing, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogError("[Home Sharing] Failed to leave home sharing for user {userId} in home {homeId}", request.UserId, request.HomeId);
            return Result.Failure(result.Errors);
        }

        logger.LogInformation("[Home Sharing] User {userId} left home {homeId} sharing.", request.UserId, request.HomeId);
        return Result.Success();
    }
}
