using Senswave.Devices.Domain.Devices.Repositories;
using Senswave.Devices.Domain.Devices.Services;
using Senswave.Integration.Homes.CanDisplayHome;
using Senswave.Integration.Homes.CanManageHome;

namespace Senswave.Devices.Application.Devices.Services;

public class DeviceAccessService(
    IRequestClient<CanDisplayHomeRequest> displayHomeClient,
    IRequestClient<CanManageHomeRequest> manageHomeClient,
    IDeviceQueryRepository queryRepository,
    ILogger<DeviceAccessService> logger) : IDeviceAccessService
{
    #region Errors

    private readonly Error UserHasNoAccess = Error.Failure("UserHasNoAccessToDevice", "User does not have access to this device.");

    #endregion

    public async Task<Result> IsOwner(Guid userId, Guid deviceId, CancellationToken cancellationToken)
    {
        var isOwner = await queryRepository.IsDeviceOwner(userId, deviceId, cancellationToken);

        if (isOwner)
        {
            logger.LogInformation("[User: {UserId}] is the owner of device: {DeviceId}.", userId, deviceId);
            return Result.Success();
        }

        logger.LogWarning("[User: {UserId}] is not the owner of device: {DeviceId}.", userId, deviceId);
        return Result.Failure(UserHasNoAccess);
    }

    public async Task<Result> CanDisplayHome(Guid userId, Guid homeId, CancellationToken cancellationToken)
    {
        var request = new CanDisplayHomeRequest
        {
            UserId = userId,
            HomeId = homeId
        };

        var result = await displayHomeClient.GetResponse<CanDisplayHomeResponse>(request, cancellationToken);

        if (result.Message.IsSuccess)
        {
            logger.LogInformation("[User: {UserId}] has access to display home: {HomeId}.", userId, homeId);
            return Result.Success();
        }

        logger.LogWarning("[User: {UserId}] has no access to display home: {HomeId}.", userId, homeId);
        return Result.Failure(UserHasNoAccess);
    }

    public async Task<Result> CanManageHome(Guid userId, Guid homeId, CancellationToken cancellationToken)
    {
        var request = new CanManageHomeRequest
        {
            UserId = userId,
            HomeId = homeId
        };

        var result = await manageHomeClient.GetResponse<CanManageHomeResponse>(request, cancellationToken);

        if (result.Message.IsSuccess)
        {
            logger.LogInformation("[User: {UserId}] has access to manage home: {HomeId}.", userId, homeId);
            return Result.Success();
        }

        logger.LogWarning("[User: {UserId}] has no access to manage home: {HomeId}.", userId, homeId);
        return Result.Failure(UserHasNoAccess);
    }

    public async Task<Result> CanDisplay(Guid userId, Guid deviceId, CancellationToken cancellationToken)
    {
        var isOwner = await queryRepository.IsDeviceOwner(userId, deviceId, cancellationToken);

        if (isOwner)
        {
            logger.LogInformation("[User: {UserId}] is the owner of device: {DeviceId}.", userId, deviceId);
            return Result.Success();
        }

        var isOverrided = await queryRepository.IsDeviceShared(userId, deviceId, cancellationToken);

        if (!isOverrided)
        {
            var homeId = await queryRepository.GetHomeIdByDevice(deviceId, cancellationToken);

            if (homeId == Guid.Empty)
            {
                logger.LogWarning("[User: {UserId}] has no access to device: {DeviceId} because it is not assigned to any home.", userId, deviceId);
                return Result.Failure(UserHasNoAccess);
            }

            return await CanDisplayHome(userId, homeId, cancellationToken);
        }

        var hasAccess = await queryRepository.HasDisplayAccessToDevice(userId, deviceId, cancellationToken);

        if (!hasAccess)
        {
            logger.LogWarning("[User: {UserId}] has no access to display device: {DeviceId}.", userId, deviceId);
            return Result.Failure(UserHasNoAccess);
        }

        logger.LogInformation("[User: {UserId}] has access to display device: {DeviceId}.", userId, deviceId);
        return Result.Success();
    }

    public async Task<Result> CanAct(Guid userId, Guid deviceId, CancellationToken cancellationToken)
    {
        var isOwner = await queryRepository.IsDeviceOwner(userId, deviceId, cancellationToken);

        if (isOwner)
        {
            logger.LogInformation("[User: {UserId}] is the owner of device: {DeviceId}.", userId, deviceId);
            return Result.Success();
        }

        var isOverrided = await queryRepository.IsDeviceShared(userId, deviceId, cancellationToken);

        if (!isOverrided)
        {
            var homeId = await queryRepository.GetHomeIdByDevice(deviceId, cancellationToken);

            if (homeId == Guid.Empty)
            {
                logger.LogWarning("[User: {UserId}] has no access to device: {DeviceId} because it is not assigned to any home.", userId, deviceId);
                return Result.Failure(UserHasNoAccess);
            }

            return await CanDisplayHome(userId, homeId, cancellationToken);
        }

        var hasAccess = await queryRepository.HasActionAccessToDevice(userId, deviceId, cancellationToken);

        if (!hasAccess)
        {
            logger.LogWarning("[User: {UserId}] has no access to act on device: {DeviceId}.", userId, deviceId);
            return Result.Failure(UserHasNoAccess);
        }

        logger.LogInformation("[User: {UserId}] has access to act on device: {DeviceId}.", userId, deviceId);
        return Result.Success();
    }

    public async Task<Result> CanManage(Guid userId, Guid deviceId, CancellationToken cancellationToken)
    {
        var isOwner = await queryRepository.IsDeviceOwner(userId, deviceId, cancellationToken);

        if (isOwner)
        {
            logger.LogInformation("[User: {UserId}] is the owner of device: {DeviceId}.", userId, deviceId);
            return Result.Success();
        }

        var isOverrided = await queryRepository.IsDeviceShared(userId, deviceId, cancellationToken);

        if (!isOverrided)
        {
            var homeId = await queryRepository.GetHomeIdByDevice(deviceId, cancellationToken);

            if (homeId == Guid.Empty)
            {
                logger.LogWarning("[User: {UserId}] has no access to device: {DeviceId} because it is not assigned to any home.", userId, deviceId);
                return Result.Failure(UserHasNoAccess);
            }

            return await CanManageHome(userId, homeId, cancellationToken);
        }

        var hasAccess = await queryRepository.HasManageAccessToDevice(userId, deviceId, cancellationToken);

        if (!hasAccess)
        {
            logger.LogWarning("[User: {UserId}] has no access to manage device: {DeviceId}.", userId, deviceId);
            return Result.Failure(UserHasNoAccess);
        }

        logger.LogInformation("[User: {UserId}] has access to manage device: {DeviceId}.", userId, deviceId);
        return Result.Success();
    }
}
