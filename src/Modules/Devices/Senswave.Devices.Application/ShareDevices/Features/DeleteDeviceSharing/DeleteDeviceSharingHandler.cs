using Senswave.Devices.Domain.Devices.Services;
using Senswave.Devices.Domain.ShareDevices.Repositories;

namespace Senswave.Devices.Application.ShareDevices.Features.DeleteDeviceSharing;

public class DeleteDeviceSharingHandler(
    IDeviceAccessService accessService,
    IDeviceSharingCommandRepository sharingRepository,
    ILogger<DeleteDeviceSharingHandler> logger)
    : ICommandHandler<DeleteDeviceSharingCommand>
{
    public async Task<Result> Handle(DeleteDeviceSharingCommand request, CancellationToken cancellationToken)
    {
        var deviceSharing = await sharingRepository.GetDeviceSharing(request.DeviceSharingId, cancellationToken);

        if (deviceSharing == null)
        {
            logger.LogWarning("[User: {UserId}] Device sharing not found: {DeviceSharingId}", request.UserId, request.DeviceSharingId);
            return Result.Failure(DeleteDeviceSharingErrors.DeviceSharingNotFound);
        }

        var isOwner = await accessService.IsOwner(request.UserId, deviceSharing.DeviceId, cancellationToken);

        if (!isOwner)
            return Result.Failure(isOwner.Errors);

        var result = await sharingRepository.DeleteDeviceSharing(deviceSharing, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogError("[User: {UserId}] Failed to delete device sharing: {DeviceSharingId}", request.UserId, request.DeviceSharingId);
            return Result.Failure(result.Errors);
        }

        logger.LogInformation("[User: {UserId}] Device sharing deleted successfully: {DeviceSharingId}", request.UserId, request.DeviceSharingId);
        return Result.Success();
    }
}