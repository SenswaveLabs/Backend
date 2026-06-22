using Senswave.Devices.Domain.Devices.Services;
using Senswave.Devices.Domain.Operations.Repositories;
using Senswave.Devices.Domain.Operations.Services;

namespace Senswave.Devices.Application.Operations.Services;

public class OperationAccessService(
    IOperationQueryRepository repository,
    IDeviceAccessService deviceAccessService,
    ILogger<OperationAccessService> logger) : IOperationAccessService
{
    #region Errors

    private readonly Error OperationNotFound = Error.NotFound("OperationNotFound", "Operation not found.");

    #endregion

    public async Task<Result> CanDisplay(Guid userId, Guid operationId, CancellationToken cancellationToken)
    {
        var deviceId = await repository.GetDeviceIdByOperationId(operationId, cancellationToken);

        if (deviceId == default)
        {
            logger.LogWarning("[User: {userId}] [Operation: {operationId}] Device not found for operation.", userId, operationId);
            return Result.Failure(OperationNotFound);
        }

        return await deviceAccessService.CanDisplay(userId, deviceId, cancellationToken);
    }

    public Task<Result> CanDisplayDevice(Guid userId, Guid deviceId, CancellationToken cancellationToken)
        => deviceAccessService.CanDisplay(userId, deviceId, cancellationToken);

    public async Task<Result> CanManage(Guid userId, Guid operationId, CancellationToken cancellationToken)
    {
        var deviceId = await repository.GetDeviceIdByOperationId(operationId, cancellationToken);

        if (deviceId == default)
        {
            logger.LogWarning("[User: {userId}] [Operation: {operationId}] Device not found for operation.", userId, operationId);
            return Result.Failure(OperationNotFound);
        }

        return await deviceAccessService.CanManage(userId, deviceId, cancellationToken);
    }

    public Task<Result> CanManageDevice(Guid userId, Guid deviceId, CancellationToken cancellationToken)
        => deviceAccessService.CanManage(userId, deviceId, cancellationToken);
}
