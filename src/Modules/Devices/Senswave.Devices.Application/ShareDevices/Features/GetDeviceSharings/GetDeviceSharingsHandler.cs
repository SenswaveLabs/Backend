using Senswave.Devices.Application.ShareDevices.Features.SetDeviceSharing;
using Senswave.Devices.Domain.Devices.Services;
using Senswave.Devices.Domain.ShareDevices.Enums;
using Senswave.Devices.Domain.ShareDevices.Repositories;
using Senswave.Integration.Homes.HomeUsers;
using Senswave.Integration.User;

namespace Senswave.Devices.Application.ShareDevices.Features.GetDeviceSharings;

public class GetDeviceSharingsHandler(
    IDeviceAccessService accessService,
    IDeviceSharingQueryRepository repository,
    IRequestClient<EmailRequest> userClient,
    IRequestClient<HomeUsersRequest> homeUsersClient,
    ILogger<GetDeviceSharingsHandler> logger) : IQueryHandler<GetDeviceSharingsQuery, List<DeviceSharingModel>>
{
    public async Task<Result<List<DeviceSharingModel>>> Handle(GetDeviceSharingsQuery request, CancellationToken cancellationToken)
    {
        var isOwner = await accessService.IsOwner(request.UserId, request.DeviceId, cancellationToken);

        if (!isOwner)
            return Result<List<DeviceSharingModel>>.Failure(SetDeviceSharingsErrors.NoAccess);

        var device = await repository.GetDevice(request.DeviceId, cancellationToken);

        if (device is null)
        {
            logger.LogWarning("[User: {UserId}] Device not found: {DeviceId}", request.UserId, request.DeviceId);
            return Result<List<DeviceSharingModel>>.Failure(SetDeviceSharingsErrors.DeviceNotFound);
        }

        var usersRequest = new HomeUsersRequest
        {
            HomeId = device.HomeReference.HomeId
        };

        var homeUsers = await homeUsersClient.GetResponse<HomeUsersResponse>(usersRequest, cancellationToken);

        if (homeUsers.Message.IsFailure)
        {
            logger.LogWarning("[User: {UserId}] Failed to get home users for home: {HomeId}", request.UserId, device.HomeReference.HomeId);
            return Result<List<DeviceSharingModel>>.Failure(SetDeviceSharingsErrors.FailedToGetHomeUsers);
        }

        if (homeUsers.Message.Users.Count == 0)
        {
            logger.LogWarning("[User: {UserId}] Home not shared: {HomeId}", request.UserId, device.HomeReference.HomeId);
            return Result<List<DeviceSharingModel>>.Failure(SetDeviceSharingsErrors.HomeNotShared);
        }

        var emailsRequest = new EmailRequest
        {
            UserIds = homeUsers.Message.Users
            .Select(x => x.UserId)
            .ToList()
        };

        var emailsResponse = await userClient.GetResponse<EmailResponse>(emailsRequest, cancellationToken);

        var overridenDeviceSharings = await repository.GetSharingsByDeviceId(request.DeviceId, cancellationToken);

        var deviceSharingDtos = new List<DeviceSharingModel>();

        foreach (var user in homeUsers.Message.Users)
        {
            var email = "Unknown";

            if (emailsResponse.Message.UserEmails.TryGetValue(user.UserId, out var value))
                email = value ?? email;

            var sharingType = MapSharingType(user.HomeSharingType);
            Guid? sharingTypeId = null;

            if (overridenDeviceSharings.Any(x => x.UserId == user.UserId))
            {
                var overridenSharing = overridenDeviceSharings.First(x => x.UserId == user.UserId);
                sharingType = overridenSharing.SharingType;
                sharingTypeId = overridenSharing.Id;
            }

            deviceSharingDtos.Add(new DeviceSharingModel
            {
                SharingId = sharingTypeId,
                SharingType = sharingType,
                FriendEmail = email
            });
        }

        logger.LogInformation("[User: {UserId}] Retrieved {Count} device sharings for device: {DeviceId}",
            request.UserId,
            deviceSharingDtos.Count,
            request.DeviceId);

        return Result<List<DeviceSharingModel>>.Success(deviceSharingDtos);
    }

    private static DeviceSharingType MapSharingType(string homeSharingType) => homeSharingType switch
    {
        "Manage" => DeviceSharingType.Manage,
        "Display" => DeviceSharingType.Action,
        _ => DeviceSharingType.Invalid
    };
}