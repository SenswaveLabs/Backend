using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Operations.Enums;
using Senswave.Devices.Domain.Operations.Types;
using Senswave.Devices.Domain.Operations.Types.Boolean;
using Senswave.Devices.Domain.Operations.Types.HexColor;
using Senswave.Devices.Domain.Operations.Types.Integer;
using Senswave.Devices.Domain.Operations.Types.Number;
using Senswave.Devices.Domain.Operations.Types.Option;
using Senswave.Devices.Domain.Operations.Types.Text;

namespace Senswave.Devices.Domain.Operations.Factory;

public sealed class OperationFactory(
    ILogger<IOperation> operationLogger,
    ILogger<OperationFactory> logger)
{
    public async Task<Result<IOperation>> Initialize(
        Guid deviceId,
        Guid dataSourceReferenceId,
        string name,
        OperationType type,
        JsonObject configuration,
        CancellationToken cancellationToken)
    {
        var operation = new Operation
        {
            DeviceId = deviceId,

            DataReferenceId = dataSourceReferenceId,

            Name = name,
            Type = type,
            Configuration = configuration,
        };

        var parsedOperationResult = Create(operation);

        if (parsedOperationResult.IsFailure)
        {
            logger.LogError("[Operation: {operationId}] [OperationType: {operationType}] Failed to initialize operation.",
                operation.Id,
                operation.Type);

            return Result<IOperation>.Failure(Error.Failure("FailedToInitializeOperation", "Failed to initialize operation."));
        }

        var validationResult = await parsedOperationResult.Data
             .Validate(cancellationToken);

        if (validationResult.IsFailure)
        {
            logger.LogError("[Operation: {operationId}] [OperationType: {operationType}] Failed to validate operation.",
                operation.Id,
                operation.Type);

            return Result<IOperation>.Failure(Error.Validation("OperationValidationFailed", "Failed to validate operation"));
        }

        return Result<IOperation>.Success(parsedOperationResult.Data);
    }

    public Result<IOperation> Create(Operation operation)
    {
        try
        {
            var result = operation.Type switch
            {
                OperationType.Boolean => Result<IOperation>.Success(operation.AsBooleanOperation(operationLogger)),
                OperationType.Integer => Result<IOperation>.Success(operation.AsIntegerOperation(operationLogger)),
                OperationType.Number => Result<IOperation>.Success(operation.AsNumberOperation(operationLogger)),
                OperationType.Text => Result<IOperation>.Success(operation.AsTextOperation(operationLogger)),
                OperationType.Options => Result<IOperation>.Success(operation.AsOptionsOperation(operationLogger)),
                OperationType.HexColor => Result<IOperation>.Success(operation.AsHexColorOperation(operationLogger)),
                _ => Result<IOperation>.Failure(Error.Failure("InvalidOperationType", "Cannot create operation implementation.")),
            };

            if (result.IsFailure)
            {
                logger.LogError("[Operation: {operationId}] [OperationType: {operationType}] Failed to create operation implementation.",
                    operation.Id,
                    operation.Type);
            }

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Operation: {operationId}] [OperationType: {operationType}] Failed to create operation implementation.",
                operation.Id,
                operation.Type);

            return Result<IOperation>.Failure(Error.Failure("FailedToCreateOperation", "Failed to create operation."));
        }
    }
}
