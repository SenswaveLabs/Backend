using Senswave.Devices.Domain.Devices.Services;
using Senswave.Devices.Domain.ShareDevices.Repositories;
using Senswave.Integration.Homes.HomeShared;
using Senswave.Integration.User;

namespace Senswave.Devices.Application.ShareDevices.Features.SetDeviceSharing;

public class SetDeviceSharingsHandler(
    IDeviceAccessService accessService,
    IRequestClient<UserByEmailRequest> userIdRequestClient,
    IRequestClient<HomeSharedRequest> isHomeSharedRequestClient,
    IDeviceSharingCommandRepository commandRepository,
    IDeviceSharingQueryRepository queryRepository,
    ILogger<SetDeviceSharingsHandler> logger) : ICommandHandler<SetDeviceSharingsCommand>
{
    public async Task<Result> Handle(SetDeviceSharingsCommand request, CancellationToken cancellationToken)
    {
        var isOwner = await accessService.IsOwner(request.UserId, request.DeviceId, cancellationToken);

        if (!isOwner)
            return Result.Failure(SetDeviceSharingsErrors.NoAccess);

        var friendIdRequest = new UserByEmailRequest
        {
            Email = request.FriendEmail
        };

        var friendIdResponse = await userIdRequestClient.GetResponse<UserByEmailResponse>(friendIdRequest, cancellationToken);

        if (friendIdResponse.Message.IsFailure)
        {
            logger.LogWarning("[User: {UserId}] Failed to find user by email.", request.UserId);
            return Result.Failure(SetDeviceSharingsErrors.UserNotFound);
        }

        if (friendIdResponse.Message.UserId == request.UserId)
        {
            logger.LogWarning("[User: {UserId}] Attempted to share device with self.", request.UserId);
            return Result.Failure(SetDeviceSharingsErrors.SelfSharing);
        }

        var device = await queryRepository.GetDevice(request.DeviceId, cancellationToken);

        if (device is null)
        {
            logger.LogWarning("[User: {UserId}] Device not found: {DeviceId}", request.UserId, request.DeviceId);
            return Result.Failure(SetDeviceSharingsErrors.DeviceNotFound);
        }

        var homeSharedRequest = new HomeSharedRequest
        {
            UserId = friendIdResponse.Message.UserId,
            HomeId = device.HomeReference.HomeId
        };

        var isHomeSharedRequest = await isHomeSharedRequestClient.GetResponse<HomeSharedResponse>(homeSharedRequest, cancellationToken);

        if (isHomeSharedRequest.Message.IsFailure)
        {
            logger.LogWarning("[User: {UserId}] Failed to check if home is shared for user: {UserId}", request.UserId, friendIdResponse.Message.UserId);
            return Result.Failure(SetDeviceSharingsErrors.HomeNotShared);
        }

        var result = await commandRepository.CreateOrUpdateDeviceSharing(friendIdResponse.Message.UserId, request.DeviceId, request.SharingType, cancellationToken);

        if (!result)
        {
            logger.LogError("[User: {UserId}] [Device: {DeviceId}] Failed to create or update device sharing for user: {UserId}",
                request.UserId,
                request.DeviceId,
                friendIdResponse.Message.UserId);

            return Result.Failure(SetDeviceSharingsErrors.SharingCreationFailed);
        }

        logger.LogInformation("[User: {UserId}] [Device: {DeviceId}] Device sharing updated successfully for user: {UserId}",
            request.UserId,
            request.DeviceId,
            friendIdResponse.Message.UserId);

        return Result.Success();
    }
}