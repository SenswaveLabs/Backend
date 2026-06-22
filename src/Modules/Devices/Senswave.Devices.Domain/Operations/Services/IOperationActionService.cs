using Senswave.Devices.Domain.Operations.Models;

namespace Senswave.Devices.Domain.Operations.Services;

public interface IOperationActionService
{
    Task<Result<List<Guid>>> IncomingOperationActionProcessing(
        Guid dataSourceReferenceId,
        string payload,
        CancellationToken cancellationToken);

    Task<Result<OperationAssembledModel>> OperationActionWithEvent(
        Guid operationId,
        JsonValue value,
        CancellationToken cancellationToken);

    Task<Result<OperationAssembledModel>> OperationAction(
        Guid operationId,
        JsonValue value,
        CancellationToken cancellationToken);
}
