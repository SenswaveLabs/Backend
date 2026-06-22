using Senswave.Abstractions.Messaging.Interfaces;
using Senswave.Devices.Domain.Devices.Repositories;
using Senswave.Devices.Domain.Devices.Services;
using Senswave.Devices.Domain.Operations.Services;
using Senswave.Devices.Domain.Services;
using Senswave.Devices.Domain.Widgets.Repositories;
using Senswave.Devices.Domain.Widgets.Services;
using Senswave.Integration.DataTransfer.Devices;
using System.Text.Json.Nodes;

namespace Senswave.Devices.Application.Services;

internal class ActionService(
        IWidgetQueryRepository widgetQueryRepository,
        IWidgetService widgetService,
        IDeviceService deviceService,
        IDeviceQueryRepository deviceQueryRepository,
        IOperationActionService operationActionService,
        IPublishMessageBus bus,
        ILogger<ActionService> logger
        ) : IActionService
{
    public async Task<Result> IncomingActionProcessing(List<Guid> operationIds, CancellationToken cancellationToken)
    {
        logger.LogInformation("Sending update events after processing.");

        await deviceService.DevicePresenceEvent(operationIds, cancellationToken);
        await DeviceTileActionEvent(operationIds, cancellationToken);
        await WidgetActionEvent(operationIds, cancellationToken);

        return Result.Success();
    }

    public async Task<Result> TileAction(Guid deviceId, JsonValue value, CancellationToken cancellationToken)
    {
        logger.LogDebug("[Device: {deviceId}]Starting device tile action.", deviceId);

        var presence = await deviceService.IsDevicePresent(deviceId, cancellationToken);

        if (presence.IsFailure)
            return Result.Failure(presence.Errors);

        var assembingResult = await deviceService.PreprocessValueForTileMessage(deviceId, value, cancellationToken);

        if (!assembingResult)
            return Result.Failure(assembingResult.Errors);

        var result = await operationActionService.OperationActionWithEvent(assembingResult.Data.OperationId, assembingResult.Data.Value, cancellationToken);

        if (!result)
        {
            logger.LogError("[Device: {deviceId}] Failed to send tile message to device.", deviceId);
            return Result.Failure(result.Errors);
        }

        if (result.Data.SendEvents)
        {
            logger.LogInformation("[Device: {deviceId}] Executing events.",
                deviceId);

            await DeviceTileActionEvent(deviceId, cancellationToken);
            await WidgetActionEvent([assembingResult.Data.OperationId], cancellationToken);
        }

        logger.LogInformation("[Device: {deviceId}] Device tile action finished.", deviceId);
        return Result.Success();
    }

    public async Task<Result> WidgetAction(Guid widgetId, JsonValue value, CancellationToken cancellationToken)
    {
        logger.LogDebug("[Widget: {widgetId}] Starting widget action.", widgetId);

        var presence = await deviceService.IsDevicePresentByWidgetId(widgetId, cancellationToken);

        if (presence.IsFailure)
            return Result<Guid>.Failure(presence.Errors);

        var preprocessingResult = await widgetService.PreprocessValueForWidgetMessage(widgetId, value, cancellationToken);

        if (preprocessingResult.IsFailure)
            return Result<Guid>.Failure(preprocessingResult.Errors);

        var result = await operationActionService
            .OperationActionWithEvent(preprocessingResult.Data.OperationId, preprocessingResult.Data.Value, cancellationToken);

        if (result.IsFailure)
            return Result<Guid>.Failure(result.Errors);

        if (result.Data.SendEvents)
        {
            logger.LogInformation("[Widget: {widgetId}] Executing widget events.",
                widgetId);

            await WidgetActionEvent([preprocessingResult.Data.OperationId], cancellationToken);
            await DeviceTileActionEvent([preprocessingResult.Data.OperationId], cancellationToken);
        }

        logger.LogInformation("[Widget: {widgetId} Widget action finished.", widgetId);
        return Result.Success();
    }

    public async Task<Result> ExternalAction(Guid operationId, JsonValue value, CancellationToken cancellationToken)
    {
        logger.LogDebug("[Operation: {operationId}] Starting external action.", operationId);

        var presence = await deviceService.IsDevicePresentByOperationId(operationId, cancellationToken);

        if (presence.IsFailure)
            return presence;

        var result = await operationActionService.OperationAction(operationId, value, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogInformation("[Operation: {operationId}] External action failed.", operationId);
            return Result<Guid>.Failure(result.Errors);
        }

        if (result.Data.SendEvents)
        {
            logger.LogDebug("[Operation: {operationId}] Sending events after external action.", operationId);

            await DeviceTileActionEvent([operationId], cancellationToken);
            await WidgetActionEvent([operationId], cancellationToken);
        }

        logger.LogInformation("[Operation: {operationId}] External action finished.", operationId);
        return Result.Success();
    }


    #region DeviceTile

    private async Task DeviceTileActionEvent(List<Guid> operationIds, CancellationToken cancellationToken)
    {
        var device = await deviceQueryRepository.GetDeviceByOperationsIfTileOperationAction(operationIds, cancellationToken);

        if (device is null)
        {
            logger.LogWarning("[Operations: {operationIds}] Failed to find device for operations or operations are not tile action type.",
                string.Join(", ", operationIds));

            return;
        }

        await DeviceTileActionEvent(device.Id, cancellationToken);
    }

    private async Task DeviceTileActionEvent(Guid deviceId, CancellationToken cancellationToken)
    {
        var message = new DeviceTileActionEvent()
        {
            DeviceId = deviceId
        };

        await bus.Publish(message, cancellationToken);
    }

    #endregion

    #region Widgets

    private async Task WidgetActionEvent(List<Guid> operationIds, CancellationToken cancellationToken)
    {
        if (operationIds.Count == 0)
        {
            logger.LogDebug("No operation Ids provided for widget action event.");
            return;
        }

        var deviceId = await widgetQueryRepository.GetDeviceIdByOperations(operationIds, cancellationToken);

        if (deviceId == Guid.Empty)
        {
            logger.LogError("Failed to find device for operations: {operationIds}", string.Join(", ", operationIds));
            return;
        }

        var widgetIds = await widgetQueryRepository.GetWidgetsByOperationIds(operationIds, cancellationToken);

        var message = new WidgetActionEvent()
        {
            DeviceId = deviceId,
            WidgetIds = widgetIds
        };

        logger.LogInformation("[Device: {deviceId}] Publishing widget action event for {Count} operations.",
            deviceId, operationIds.Count);

        await bus.Publish(message, cancellationToken);
    }

    #endregion

    #region External 

    #endregion
}
