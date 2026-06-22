using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Devices.Repositories;
using Senswave.Devices.Domain.Devices.Services;

namespace Senswave.Devices.Application.Devices.Features.GetDevice;

public class GetDeviceHandler(
    IDeviceAccessService accessService,
    IDeviceQueryRepository repository,
    ILogger<GetDeviceHandler> logger)
    : IQueryHandler<GetDeviceQuery, Device>
{
    public async Task<Result<Device>> Handle(GetDeviceQuery request, CancellationToken cancellationToken)
    {
        var canDisplay = await accessService.CanDisplay(request.UserId, request.DeviceId, cancellationToken);

        if (!canDisplay)
        {
            logger.LogWarning("[UserId: {UserId}][DeviceId: {DeviceId}] User has no access to device.", request.UserId, request.DeviceId);
            return Result<Device>.Failure(GetDeviceErrors.NoAccess, canDisplay.Errors);
        }

        var device = await repository.GetDevice(request.DeviceId, cancellationToken);

        if (device is null)
        {
            logger.LogWarning("[UserId: {UserId}][DeviceId: {DeviceId}] Device not found.", request.UserId, request.DeviceId);
            return Result<Device>.Failure(GetDeviceErrors.NotFound);
        }

        logger.LogInformation("[UserId: {UserId}][DeviceId: {DeviceId}] Device retrieved successfully.", request.UserId, request.DeviceId);
        return Result<Device>.Success(device);
    }
}