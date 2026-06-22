using Senswave.Devices.Domain.Devices.Models;
using Senswave.Devices.Domain.Devices.Repositories;
using Senswave.Devices.Domain.Devices.Services;

namespace Senswave.Devices.Application.Devices.Features.DisplayDevice;

internal sealed class DisplayDeviceHandler(
    IDeviceAccessService accessService,
    IDeviceQueryRepository queryRepository,
    IDeviceService interpreter,
    ILogger<DisplayDeviceHandler> logger) : IQueryHandler<DisplayDeviceQuery, DisplayDeviceModel>
{
    public async Task<Result<DisplayDeviceModel>> Handle(DisplayDeviceQuery request, CancellationToken cancellationToken)
    {
        var canDisplay = await accessService.CanDisplay(request.UserId, request.DeviceId, cancellationToken);

        if (!canDisplay)
        {
            logger.LogWarning("[UserId: {UserId}][DeviceId: {DeviceId}] User has no access to device.", request.UserId, request.DeviceId);
            return Result<DisplayDeviceModel>.Failure(DisplayDeviceErrors.NoAccess, canDisplay.Errors);
        }

        var device = await queryRepository.GetDevice(request.DeviceId, cancellationToken);

        if (device is null)
        {
            logger.LogWarning("[UserId: {UserId}][DeviceId: {DeviceId}] Device not found.", request.UserId, request.DeviceId);
            return Result<DisplayDeviceModel>.Failure(DisplayDeviceErrors.NotFound);
        }

        var interpreted = await interpreter.Interpret(device);

        if (interpreted.IsFailure)
        {
            logger.LogError("[UserId: {UserId}][DeviceId: {DeviceId}] Failed to interpret device. Errors: {Errors}", request.UserId, request.DeviceId, interpreted.Errors);
            return interpreted;
        }

        logger.LogInformation("[UserId: {UserId}][DeviceId: {DeviceId}] Device interpreted successfully.", request.UserId, request.DeviceId);
        return interpreted;
    }
}
