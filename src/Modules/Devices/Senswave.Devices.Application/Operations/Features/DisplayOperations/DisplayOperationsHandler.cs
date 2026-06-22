using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Operations.Repositories;
using Senswave.Devices.Domain.Operations.Services;

namespace Senswave.Devices.Application.Operations.Features.DisplayOperations;

public class DisplayOperationsHandler(
    IOperationAccessService accessService,
    IOperationQueryRepository repository,
    ILogger<DisplayOperationsHandler> logger) : IQueryHandler<DisplayOperationsQuery, IEnumerable<Operation>>
{
    public async Task<Result<IEnumerable<Operation>>> Handle(DisplayOperationsQuery request, CancellationToken cancellationToken)
    {
        var canDisplay = await accessService.CanDisplayDevice(request.UserId, request.DeviceId, cancellationToken);

        if (!canDisplay)
            return Result<IEnumerable<Operation>>.Failure(canDisplay.Errors);

        var operations = await repository.GetOperations(request.DeviceId, request.Page, request.Size, cancellationToken);

        if (operations.Count == 0)
        {
            logger.LogWarning("[User: {UserId}] No operations found for device: {DeviceId}.",
                request.UserId,
                request.DeviceId);
            return Result<IEnumerable<Operation>>.Failure(DisplayOperationsErrors.OperationsNotFound);
        }

        logger.LogInformation("[User: {UserId}] Retrieved {Count} operations for device: {DeviceId}.",
            request.UserId,
            operations.Count,
            request.DeviceId);
        return Result<IEnumerable<Operation>>.Success(operations);
    }
}