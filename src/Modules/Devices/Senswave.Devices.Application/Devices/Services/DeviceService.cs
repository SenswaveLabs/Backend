using Senswave.Abstractions.Messaging.Interfaces;
using Senswave.Devices.Application.Devices.Errors;
using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Devices.Enums;
using Senswave.Devices.Domain.Devices.Factory;
using Senswave.Devices.Domain.Devices.Models;
using Senswave.Devices.Domain.Devices.Repositories;
using Senswave.Devices.Domain.Devices.Services;
using Senswave.Devices.Domain.Operations.Extensions;
using Senswave.Devices.Domain.Operations.Factory;
using Senswave.Integration.DataTransfer.Devices;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Senswave.Devices.Application.Devices.Services;

public sealed class DeviceService(
    OperationFactory operationFactory,
    DeviceFactory deviceFactory,
    IPublishMessageBus bus,
    IDeviceQueryRepository devicesQueryRepository,
    ILogger<DeviceService> logger) : IDeviceService
{
    public async Task<Result> IsDevicePresentByWidgetId(Guid widgetId, CancellationToken cancellationToken)
    {
        var deviceId = await devicesQueryRepository.GetDeviceIdByWidgetId(widgetId, cancellationToken);

        if (deviceId == Guid.Empty)
        {
            logger.LogCritical("[Widget: {widgetId}] Device was not found for widget.", widgetId);
            return Result<DisplayDeviceModel>.Failure(DeviceServiceErrors.FailedToFindDevice);
        }

        return await IsDevicePresent(deviceId, cancellationToken);
    }

    public async Task<Result> IsDevicePresentByOperationId(Guid operationId, CancellationToken cancellationToken)
    {
        var deviceId = await devicesQueryRepository.GetDeviceIdByOperationId(operationId, cancellationToken);

        if (deviceId == Guid.Empty)
        {
            logger.LogCritical("[Operation: {operationId}] Device was not found for operation.", operationId);
            return Result<DisplayDeviceModel>.Failure(DeviceServiceErrors.FailedToFindDevice);
        }

        return await IsDevicePresent(deviceId, cancellationToken);
    }

    public async Task<Result> IsDevicePresent(Guid deviceId, CancellationToken cancellationToken)
    {
        var device = await devicesQueryRepository.GetDeviceForOnlineStatus(deviceId, cancellationToken);

        if (device is null)
        {
            logger.LogCritical("[Device: {deviceId}] Device was not found.", deviceId);
            return Result<DisplayDeviceModel>.Failure(DeviceServiceErrors.FailedToFindDevice);
        }

        return await IsDevicePresent(device);
    }

    private async Task<Result> IsDevicePresent(Device device)
    {
        if (device.Presence is null)
        {
            logger.LogWarning("[Device: {deviceId}] Device does not have presence information.", device.Id);
            return Result.Success();
        }

        if (device.Presence.OperationId is null || device.Presence.Operation is null)
        {
            logger.LogInformation("[Device: {deviceId}] Device presence type.", device.Id);
            return Result.Success();
        }

        var operation = operationFactory.Create(device.Presence.Operation!);

        if (operation.IsFailure)
        {
            logger.LogError("[Device: {deviceId}] Failed to create operation for device presence.", device.Id);
            return Result<DisplayDeviceModel>.Failure(DeviceServiceErrors.FailedToCreateOperationForDevicePresence);
        }

        var currentValueResult = await operation.Data.GetCurrentValue();

        if (currentValueResult.IsSuccess && currentValueResult.Data.Value.GetValueKind() == JsonValueKind.True)
        {
            logger.LogInformation("[Device: {deviceId}] Device is present.", device.Id);
            return Result.Success();
        }

        logger.LogInformation("[Device: {deviceId}] Device is not present.", device.Id);
        return Result.Failure(DeviceServiceErrors.DeviceIsNotPresent);
    }

    public async Task<Result<DisplayDeviceModel>> Interpret(Guid deviceId, CancellationToken cancellationToken)
    {
        var deviceResult = await deviceFactory.Create(deviceId, cancellationToken);

        if (deviceResult.IsFailure)
        {
            logger.LogWarning("[Device: {deviceId}] Device not found.", deviceId);
            return Result<DisplayDeviceModel>.Failure(DeviceServiceErrors.FailedToFindDevice);
        }

        logger.LogInformation("[Device: {deviceId}] Successfully interpreted device.", deviceId);
        return await deviceResult.Data.ToDisplay();
    }

    public async Task<Result<DisplayDeviceModel>> Interpret(Device device)
    {
        var deviceResult = deviceFactory.Create(device);

        if (deviceResult.IsFailure)
        {
            logger.LogError("[Device: {deviceId}] Unsupported tile type, returning invalid.", device.Id);
            return Result<DisplayDeviceModel>.Success(new DisplayDeviceModel
            {
                Id = device.Id,
                RoomId = device.RoomReferenceId,
                Name = device.Name,
                Icon = device.Icon,
                Tile = new() { Type = DeviceTileType.Invalid },
                CreatedAtUtc = device.CreatedAtUtc,
                UpdatedAtUtc = device.UpdatedAtUtc
            });
        }

        return await deviceResult.Data.ToDisplay();
    }

    public async Task<Result<DeviceTileMessageModel>> PreprocessValueForTileMessage(Guid deviceId, JsonValue value, CancellationToken cancellationToken)
    {
        var deviceResult = await deviceFactory.Create(deviceId, cancellationToken);

        if (deviceResult.IsFailure)
        {
            logger.LogWarning("[Device: {deviceId}] Device not found.", deviceId);
            return Result<DeviceTileMessageModel>.Failure(DeviceServiceErrors.FailedToFindDevice);
        }

        return await deviceResult.Data.Tile.Preprocess(value);
    }

    public async Task DevicePresenceEvent(List<Guid> operationIds, CancellationToken cancellationToken)
    {
        if (operationIds.Count == 0)
        {
            logger.LogDebug("DevicePresenceEvent called with empty operation IDs list.");
            return;
        }

        var device = await devicesQueryRepository.GetDeviceIdByOperationIds(operationIds, cancellationToken);

        if (device is null)
        {
            logger.LogWarning("[Operations: {operationIds}] Failed to find device for operations or operations are not used in device presence.",
                string.Join(", ", operationIds));
            return;
        }

        var message = new DevicePresenceEvent()
        {
            DeviceId = device.Id
        };

        logger.LogInformation("[Device: {deviceId}] Publishing device presence event.", device.Id);

        await bus.Publish(message, cancellationToken);
    }
}
