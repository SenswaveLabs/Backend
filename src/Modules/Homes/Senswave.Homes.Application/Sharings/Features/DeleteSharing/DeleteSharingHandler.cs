using Senswave.Homes.Domain.Homes.Services;
using Senswave.Homes.Domain.Sharings.Repositories;
using Senswave.Integration.Devices.RemoveDeviceSharings;

namespace Senswave.Homes.Application.Sharings.Features.DeleteSharing;

public class DeleteSharingHandler(
    IHomeAccessService accessService,
    IRequestClient<RemoveDeviceSharingsRequest> removeDeviceSharingsClient,
    IHomeSharingCommandRepository repository,
    ILogger<DeleteSharingHandler> logger) : ICommandHandler<DeleteSharingCommand, Guid>
{
    public async Task<Result<Guid>> Handle(DeleteSharingCommand request, CancellationToken cancellationToken)
    {
        var homeSharing = await repository.GetHomeSharing(request.HomeSharingId, cancellationToken);

        if (homeSharing == null)
        {
            logger.LogWarning("[HomeSharing: {HomeSharingId}] Home sharing not found.", request.HomeSharingId);
            return Result<Guid>.Failure(DeleteSharingErrors.HomeSharingNotFound);
        }

        var isOwner = await accessService.IsOwner(request.UserId, homeSharing.HomeId, cancellationToken);

        if (!isOwner)
            return Result<Guid>.Failure(isOwner.Errors);

        var removeSharingsRequest = new RemoveDeviceSharingsRequest
        {
            UserId = homeSharing.UserId,
            HomeReferenceId = homeSharing.HomeId
        };

        var response = await removeDeviceSharingsClient.GetResponse<RemoveDeviceSharingsResponse>(removeSharingsRequest, cancellationToken);

        if (!response.Message.IsSuccess)
        {
            logger.LogError("[HomeSharing: {HomeSharingId}] Failed to remove device sharings for user {UserId}.", request.HomeSharingId, homeSharing.UserId);
            return Result<Guid>.Failure(DeleteSharingErrors.FailedToRemoveSharing);
        }

        var result = await repository.DeleteSharing(homeSharing, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogError("[HomeSharing: {HomeSharingId}] Failed to delete home sharing.", request.HomeSharingId);
            return Result<Guid>.Failure(result.Errors);
        }

        logger.LogInformation("[HomeSharing: {HomeSharingId}] Home sharing deleted successfully.", request.HomeSharingId);
        return Result<Guid>.Success(homeSharing.Id);
    }
}