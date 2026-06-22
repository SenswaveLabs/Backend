using System.Text.Json.Nodes;
using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Devices.Enums;
using Senswave.Devices.Domain.Devices.Repositories;
using Senswave.Devices.Domain.Devices.Services;
using Senswave.Devices.Domain.Operations.Enums;
using Senswave.Integration.Homes.RoomAccess;

namespace Senswave.Devices.Application.Devices.Features.UpdateDevice;

public class UpdateDeviceHandler(
    IDeviceAccessService deviceAccessService,
    IRequestClient<RoomAccessRequest> roomAccessClient,
    IDeviceQueryRepository queryRepository,
    IDeviceCommandRepository commandRepository,
    ILogger<UpdateDeviceHandler> logger) : ICommandHandler<UpdateDeviceCommand>
{
    public async Task<Result> Handle(UpdateDeviceCommand request, CancellationToken cancellationToken)
    {
        var canManage = await deviceAccessService.CanManage(request.UserId, request.DeviceId, cancellationToken);

        if (!canManage)
            return Result.Failure(UpdateDeviceErrors.DeviceNotFound, canManage.Errors);

        var device = await commandRepository.GetDeviceWithTile(request.DeviceId, cancellationToken);

        if (device is null)
        {
            logger.LogWarning("[User: {userId}] [Device: {deviceId}] Device not found.", request.UserId, request.DeviceId);
            return Result.Failure(UpdateDeviceErrors.DeviceNotFound);
        }

        if (!await NameToChange(device, request, cancellationToken))
        {
            logger.LogWarning("[User: {userId}] [Device: {deviceId}] Device name already used.", request.UserId, request.DeviceId);
            return Result.Failure(UpdateDeviceErrors.NameAlreadyUsed);
        }

        IconToChange(device, request.Icon);

        var roomToChangeSuccess = await RoomToChange(device, request, cancellationToken);
        if (!roomToChangeSuccess)
        {
            logger.LogWarning("[User: {userId}] [Device: {deviceId}] Room not found or access denied.", request.UserId, request.DeviceId);
            return Result.Failure(UpdateDeviceErrors.RoomNotFound);
        }

        // TODO: Introduce service when more tile types are added, to avoid this if-elseology - 89
        if (request.TileType == DeviceTileType.Default)
        {
            device.Tile.SwitchOperation = null;
            device.Tile.SwitchOperationId = null;
            device.Tile.DisplayableOperation = null;
            device.Tile.DisplayableOperationId = null;
            device.Tile.Type = DeviceTileType.Default;
        }
        else if (request.TileType == DeviceTileType.Switch)
        {
            var operation = device.Operations.FirstOrDefault(x => x.Id == request.TileOperationId);

            if (operation == null)
            {
                logger.LogWarning("[User: {userId}] [Device: {deviceId}] Operation not found for switch tile.", request.UserId, request.DeviceId);
                return Result.Failure(UpdateDeviceErrors.TileOperationNotFound);
            }

            if (operation.Type != OperationType.Boolean)
            {
                logger.LogWarning("[User: {userId}] [Device: {deviceId}] Invalid operation type for switch tile.", request.UserId, request.DeviceId);
                return Result.Failure(UpdateDeviceErrors.InvalidTileOperationType);
            }

            device.Tile.SwitchOperation = operation;
            device.Tile.SwitchOperationId = request.TileOperationId;
            device.Tile.Type = request.TileType;
        }
        else if (request.TileType == DeviceTileType.Display)
        {
            var operation = device.Operations.FirstOrDefault(x => x.Id == request.TileDisplayableOperationId);

            if (operation == null)
            {
                logger.LogWarning("[User: {userId}] [Device: {deviceId}] Operation not found for display tile.", request.UserId, request.DeviceId);
                return Result.Failure(UpdateDeviceErrors.TileOperationNotFound);
            }

            if (operation.Type != OperationType.Number && operation.Type != OperationType.Integer)
            {
                logger.LogWarning("[User: {userId}] [Device: {deviceId}] Invalid operation type for display tile.", request.UserId, request.DeviceId);
                return Result.Failure(UpdateDeviceErrors.InvalidDisplayTileOperationType);
            }

            device.Tile.DisplayableOperation = operation;
            device.Tile.DisplayableOperationId = request.TileDisplayableOperationId;
            if (request.TileConfiguration is not null)
                device.Tile.Configuration = JsonNode.Parse(request.TileConfiguration.ToJsonString())!.AsObject();
            device.Tile.Type = request.TileType;
        }

        // TODO: Solve with Github - 187
        bool presenceCreated = false;
        if (device.Presence is null)
        {
            device.Presence = new();
            presenceCreated = true;
        }

        if (request.PresenceType == DevicePresenceType.Default)
        {
            device.Presence.OperationId = null;
            device.Presence.Type = DevicePresenceType.Default;
        }
        else if (request.PresenceType == DevicePresenceType.BooleanOperation)
        {
            var presenceOperation = device.Operations.FirstOrDefault(x => x.Id == request.PresenceOperationId);

            if (presenceOperation == null)
            {
                logger.LogWarning("[User: {userId}] [Device: {deviceId}] Operation not found for presence.", request.UserId, request.DeviceId);
                return Result.Failure(UpdateDeviceErrors.PresenceOperationNotFound);
            }

            if (presenceOperation.Type != OperationType.Boolean)
            {
                logger.LogWarning("[User: {userId}] [Device: {deviceId}] Invalid operation type for presence.", request.UserId, request.DeviceId);
                return Result.Failure(UpdateDeviceErrors.InvalidPresenceOperationType);
            }

            device.Presence.Operation = presenceOperation;
            device.Presence.OperationId = request.PresenceOperationId;
            device.Presence.Type = request.PresenceType;
        }

        var result = Result.Failure();

        if (presenceCreated)
            result = await commandRepository.UpdateDeviceWithAddingPresence(device, cancellationToken);
        else
            result = await commandRepository.UpdateDevice(device, cancellationToken);

        if (!result)
        {
            logger.LogError("[Device: {deviceId}] Failed to update device.", request.DeviceId);
            return Result.Failure(UpdateDeviceErrors.FailedToUpdate, result.Errors);
        }

        logger.LogInformation("[User: {userId}] [Device: {deviceId}] Device updated successfully.", request.UserId, request.DeviceId);
        return Result.Success();
    }


    private async Task<bool> RoomToChange(Device device, UpdateDeviceCommand request, CancellationToken cancellationToken)
    {
        if (request.RoomId == Guid.Empty)
            return true;

        if (request.RoomId == null)
        {
            device.RoomReferenceId = null;
            return true;
        }

        var roomRequest = new RoomAccessRequest()
        {
            HomeId = device.HomeReference.HomeId,
            RoomId = request.RoomId.Value
        };

        var hasRoomAccess = await roomAccessClient.GetResponse<RoomAccessResponse>(roomRequest, cancellationToken);

        if (hasRoomAccess.Message.IsSuccess)
        {
            device.RoomReferenceId = request.RoomId;
            return true;
        }

        return false;
    }

    private async Task<bool> NameToChange(Device device, UpdateDeviceCommand request, CancellationToken cancellationToken)
    {
        if (device.Name == request.Name)
            return true;

        if (string.IsNullOrEmpty(request.Name))
            return true;

        var nameAlreadyUsedBySomeDevice = await queryRepository
            .GetDeviceByName(device.HomeReference.HomeId, request.Name, cancellationToken);

        if (nameAlreadyUsedBySomeDevice != null)
            return false;

        device.Name = request.Name;
        return true;
    }

    private static void IconToChange(Device device, string icon)
    {
        if (device.Icon != icon && !string.IsNullOrEmpty(icon))
            device.Icon = icon;
    }
}