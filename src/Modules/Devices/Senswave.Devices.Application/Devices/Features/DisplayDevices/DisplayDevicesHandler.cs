using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Devices.Models;
using Senswave.Devices.Domain.Devices.Repositories;
using Senswave.Devices.Domain.Devices.Services;

namespace Senswave.Devices.Application.Devices.Features.DisplayDevices;

public sealed class DisplayDevicesHandler(
    IDeviceAccessService accessService,
    IDeviceQueryRepository devicesQueryRepository,
    IDeviceService interpreter,
    ILogger<DisplayDevicesHandler> logger) : IQueryHandler<DisplayDevicesQuery, List<DisplayDeviceModel>>
{
    public async Task<Result<List<DisplayDeviceModel>>> Handle(DisplayDevicesQuery request, CancellationToken cancellationToken)
    {
        var canDisplay = await accessService.CanDisplayHome(request.UserId, request.HomeReferenceId, cancellationToken);

        if (!canDisplay)
        {
            logger.LogWarning("[User: {userId}][HomeReferenceId: {HomeReferenceId}] User can not get devices for home.",
                request.UserId,
                request.HomeReferenceId);

            return Result<List<DisplayDeviceModel>>.Failure(DisplayDevicesErrors.NoAccess, canDisplay.Errors);
        }

        List<Device> devices = [];

        if (request.Page != int.MaxValue && request.Size != int.MaxValue && request.Page > 0 && request.Page > 0)
        {
            devices = await devicesQueryRepository
                .GetDevices(request.HomeReferenceId, request.Page, request.Size, cancellationToken);
        }
        else
        {
            devices = await devicesQueryRepository
                .GetDevices(request.HomeReferenceId, cancellationToken);
        }


        if (devices.Count == 0)
        {
            logger.LogWarning("[UserId: {UserId}][HomeReferenceId: {HomeReferenceId}] No devices found.",
                request.UserId,
                request.HomeReferenceId);

            return Result<List<DisplayDeviceModel>>.Failure(DisplayDevicesErrors.DevicesNotFound);
        }

        var interpreted = new List<DisplayDeviceModel>();

        foreach (var device in devices)
        {
            var result = await interpreter.Interpret(device);

            if (result.IsSuccess)
                interpreted.Add(result.Data);
            else
                logger.LogError("[UserId: {UserId}][HomeReferenceId: {HomeReferenceId}] Failed to interpret device with id {DeviceId}. Errors: {Errors}",
                    request.UserId,
                    request.HomeReferenceId,
                    device.Id,
                    result.Errors);
        }

        logger.LogInformation("[UserId: {UserId}][HomeReferenceId: {HomeReferenceId}] Successfully retrieved {Count} devices.",
            request.UserId,
            request.HomeReferenceId,
            interpreted.Count);
        return Result<List<DisplayDeviceModel>>.Success(interpreted);
    }
}
