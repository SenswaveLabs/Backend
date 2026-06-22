using Senswave.Devices.Domain.Devices.Repositories;
using Senswave.Devices.Domain.Devices.Services;

namespace Senswave.Devices.Application.Devices.Features.DeleteDevice;

public class DeleteDeviceHandler(
    IDeviceAccessService accessService,
    IDeviceCommandRepository commandRepository,
    ILogger<DeleteDeviceHandler> logger) : ICommandHandler<DeleteDeviceCommand>
{
    public async Task<Result> Handle(DeleteDeviceCommand request, CancellationToken cancellationToken)
    {
        var accessResult = await accessService.IsOwner(request.UserId, request.DeviceId, cancellationToken);

        if (accessResult.IsFailure)
        {
            logger.LogWarning("[UserId: {UserId}][DeviceId: {DeviceId}] User has no access to delete device.", request.UserId, request.DeviceId);
            return Result.Failure(accessResult.Errors);
        }

        var device = await commandRepository.GetDeviceForDeletion(request.DeviceId, cancellationToken);

        if (device == null)
        {
            logger.LogWarning("[UserId: {UserId}][DeviceId: {DeviceId}] Device not found for deletion.", request.UserId, request.DeviceId);
            return Result.Failure(DeleteDeviceErrors.DeviceNotFound);
        }

        if (device.Operations.Count > 0 || device.Dashboards.Count > 0)
        {
            logger.LogWarning("[UserId: {UserId}][DeviceId: {DeviceId}] Device has operations or dashboards, cannot delete.", request.UserId, request.DeviceId);
            return Result.Failure(DeleteDeviceErrors.DeviceIsFull);
        }

        var result = await commandRepository.DeleteDevice(device, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogError("[UserId: {UserId}][DeviceId: {DeviceId}] Failed to delete device",
                request.UserId,
                request.DeviceId);
            return Result.Failure(DeleteDeviceErrors.FailedToRemove);
        }

        logger.LogInformation("[UserId: {UserId}][DeviceId: {DeviceId}] Device deleted successfully.", request.UserId, request.DeviceId);
        return Result.Success();
    }
}