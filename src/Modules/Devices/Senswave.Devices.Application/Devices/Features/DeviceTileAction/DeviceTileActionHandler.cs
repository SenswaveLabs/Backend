using Senswave.Devices.Domain.Devices.Models;
using Senswave.Devices.Domain.Devices.Services;
using Senswave.Devices.Domain.Services;

namespace Senswave.Devices.Application.Devices.Features.DeviceTileAction;

public sealed class DeviceTileActionHandler(
    IDeviceAccessService accessService,
    IActionService actionService,
    IDeviceService deviceService,
    ILogger<DeviceTileActionHandler> logger) : ICommandHandler<DeviceTileActionCommand, DisplayDeviceModel>
{
    public async Task<Result<DisplayDeviceModel>> Handle(DeviceTileActionCommand request, CancellationToken cancellationToken)
    {
        var canAct = await accessService.CanAct(request.UserId, request.DeviceId, cancellationToken);

        if (!canAct)
        {
            logger.LogError("[UserId: {UserId}][DeviceId: {DeviceId}] User has no access to device.", request.UserId, request.DeviceId);
            return Result<DisplayDeviceModel>.Failure(DeviceTileActionErrors.NoAccess, canAct.Errors);
        }

        var sendingResult = await actionService.TileAction(request.DeviceId, request.Value, cancellationToken);

        if (!sendingResult)
        {
            logger.LogError("[Device: {deviceId}] Failed to send information to device.", request.DeviceId);
            return Result<DisplayDeviceModel>.Failure(sendingResult.Errors);
        }

        logger.LogInformation("[Device: {deviceId}] Successfully send information to device.", request.DeviceId);
        return await deviceService.Interpret(request.DeviceId, cancellationToken);
    }
}
