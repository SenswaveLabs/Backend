using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Devices.Options;
using Senswave.Devices.Domain.Devices.Repositories;
using Senswave.Devices.Domain.Devices.Services;
using Senswave.Integration.Homes.Home;
using Senswave.Integration.Homes.RoomAccess;

namespace Senswave.Devices.Application.Devices.Features.CreateDevice;

public class CreateDeviceHandler(
    IDeviceAccessService accessService,
    IDeviceCommandRepository commandRepository,
    IDeviceQueryRepository queryRepository,
    IRequestClient<RoomAccessRequest> roomClient,
    IOptions<DevicesOptions> options,
    IRequestClient<HomeRequest> dataSourceRequestClient,
    ILogger<CreateDeviceHandler> logger) : ICommandHandler<CreateDeviceCommand, Device>
{
    public async Task<Result<Device>> Handle(CreateDeviceCommand request, CancellationToken cancellationToken)
    {
        var canManage = await accessService.CanManageHome(request.UserId, request.HomeId, cancellationToken);

        if (!canManage)
        {
            logger.LogInformation("[User: {userId}][Home: {homeId}] User has no access to manage home.", request.UserId, request.HomeId);
            return Result<Device>.Failure(CreateDeviceError.AccessDeniedToHome, canManage.Errors);
        }

        var homeRequest = new HomeRequest
        {
            HomeId = request.HomeId
        };

        var homeResponse = await dataSourceRequestClient.GetResponse<HomeResponse>(homeRequest, cancellationToken);

        if (homeResponse.Message.IsFailure)
        {
            logger.LogInformation("[Home: {homeId}] Home data source not found.", request.HomeId);
            return Result<Device>.Failure(CreateDeviceError.HomeNotFound);
        }

        if (homeResponse.Message.DataSourceId is null || homeResponse.Message.DataSourceId == default)
        {
            logger.LogInformation("[Home: {homeId}] Home data source not found.", request.HomeId);
            return Result<Device>.Failure(CreateDeviceError.DataSourceNotAssigned);
        }

        var deviceWithTheSameNameExists = await queryRepository.DeviceNameUsedInHome(request.HomeId, request.Name, cancellationToken);

        if (deviceWithTheSameNameExists)
            return Result<Device>.Failure(CreateDeviceError.NameAlreadyUsed);

        //TODO: Redis lock for homeId

        var devicesInHome = await queryRepository.CountDevicesForHome(request.HomeId, cancellationToken);

        if (options.Value.Limits.DevicesPerHome <= devicesInHome)
        {
            logger.LogWarning("[User: {userId}][Home: {homeId}] Limit of devices per home reached.", request.UserId, request.HomeId);
            return Result<Device>.Failure(CreateDeviceError.DeviceLimitReached);
        }

        var device = new Device()
        {
            Dashboards = [],

            Icon = request.Icon,
            Name = request.Name,

            Tile = new(),
            Presence = new(),

            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        if (request.RoomId.HasValue)
        {
            var roomRequest = new RoomAccessRequest
            {
                HomeId = request.HomeId,
                RoomId = request.RoomId.Value,
            };

            var hasRoomAccess = await roomClient.GetResponse<RoomAccessResponse>(roomRequest, cancellationToken);

            if (hasRoomAccess.Message.IsSuccess)
                device.RoomReferenceId = request.RoomId;
        }

        var result = await commandRepository.CreateDevice(
            request.HomeId,
            homeResponse.Message.OwnerId,
            device,
            cancellationToken);

        if (!result)
        {
            logger.LogError("[User: {userId}][Home: {homeId}] Failed to create device.", request.UserId, request.HomeId);
            return Result<Device>.Failure(CreateDeviceError.FailedToCreate, result.Errors);
        }

        logger.LogInformation("[User: {userId}][Home: {homeId}] Device created successfully.", request.UserId, request.HomeId);
        return Result<Device>.Success(device);
    }
}