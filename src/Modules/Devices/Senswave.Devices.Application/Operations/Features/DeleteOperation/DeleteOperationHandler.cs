using Senswave.Devices.Domain.Operations.Repositories;
using Senswave.Devices.Domain.Operations.Services;

namespace Senswave.Devices.Application.Operations.Features.DeleteOperation;

public class DeleteOperationHandler(
    IOperationAccessService accessService,
    IOperationQueryRepository queryRepository,
    IOperationCommandRepository commandRepository,
    ILogger<DeleteOperationHandler> logger) : ICommandHandler<DeleteOperationCommand>
{
    public async Task<Result> Handle(DeleteOperationCommand request, CancellationToken cancellationToken)
    {
        var canManage = await accessService.CanManage(request.UserId, request.OperationId, cancellationToken);

        if (canManage.IsFailure)
            return Result.Failure(canManage.Errors);

        var hasWidget = await queryRepository.OperationHasWidget(request.OperationId, cancellationToken);

        if (hasWidget)
        {
            logger.LogWarning("[User: {UserId}] Attempted to delete operation with widgets: {OperationId}.",
                request.UserId,
                request.OperationId);
            return Result.Failure(DeleteOperationErrors.WidgetConflict);
        }

        var usedInDeviceTile = await queryRepository.OperationHasDeviceTile(request.OperationId, cancellationToken);

        if (usedInDeviceTile)
        {
            logger.LogWarning("[User: {UserId}] Attempted to delete operation used in device tile: {OperationId}.",
                request.UserId,
                request.OperationId);
            return Result.Failure(DeleteOperationErrors.DeviceTileConfilict);
        }

        var result = await commandRepository.DeleteOperation(request.OperationId, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogError("[User: {UserId}] Failed to delete operation: {OperationId}.",
                request.UserId,
                request.OperationId);
            return Result.Failure(result.Errors);
        }

        logger.LogInformation("[User: {UserId}] Successfully deleted operation: {OperationId}.",
            request.UserId,
            request.OperationId);
        return Result.Success();
    }
}