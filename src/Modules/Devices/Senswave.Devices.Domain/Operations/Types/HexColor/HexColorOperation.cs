using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Operations.Extensions;
using Senswave.Devices.Domain.Operations.Models;
using Senswave.Devices.Domain.Operations.Types.Base;
using Senswave.Devices.Domain.Operations.Types.HexColor.Extensions;
using Senswave.Devices.Domain.Operations.ValueObjects;
using System.Text.Json;

namespace Senswave.Devices.Domain.Operations.Types.HexColor;

public class HexColorOperation(
    Operation operation,
    HexColorOperationConfiguration configuration,
    ILogger<IOperation> logger) : BaseOperation(operation, configuration)
{
    public override Result<OperationAssembledModel> CreatePayload(JsonValue value)
    {
        try
        {
            if (value.GetValueKind() != JsonValueKind.String)
                return Result<OperationAssembledModel>.Failure(InvalidDataType);

            var data = value.GetValue<string>();

            var isValid = data.IsValidHexColor();

            if (isValid.IsFailure)
            {
                logger.LogDebug("[Operation: {operationId}] [OperationType: {operationType}] Value is not valid hex color.",
                    _operation.Id,
                    _operation.Type);
                return Result<OperationAssembledModel>.Failure(ValueNotCompliant);
            }

            if (!Configuration.IsJson)
            {
                var assembled = new OperationAssembledModel
                {
                    Payload = data,
                    Value = new OperationValue
                    {
                        Operation = _operation,
                        Value = data,
                        ProcessedAtUtc = DateTime.UtcNow,
                    }
                };
                return Result<OperationAssembledModel>.Success(assembled);
            }

            var jsonPayload = configuration.ToJsonStringPayload(data);

            var assembledJson = new OperationAssembledModel
            {
                Payload = jsonPayload,
                Value = new OperationValue
                {
                    Operation = _operation,
                    Value = data,
                    ProcessedAtUtc = DateTime.UtcNow,
                }
            };

            return Result<OperationAssembledModel>.Success(assembledJson);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Operation: {operationId}] [OperationType: {operationType}] Failed to create hex color payload.",
                _operation.Id,
                _operation.Type);

            return Result<OperationAssembledModel>.Failure(FailedToCreatePayload);
        }
    }

    public override Task<Result> IsValueCompliant(JsonValue value)
    {
        try
        {
            if (value.GetValueKind() != JsonValueKind.String)
                return Task.FromResult(Result.Failure(InvalidDataType));

            var data = value.GetValue<string>();

            var isValid = data.IsValidHexColor();

            if (isValid.IsFailure)
            {
                logger.LogDebug("[Operation: {operationId}] [OperationType: {operationType}] Value is not valid hex color.",
                    _operation.Id,
                    _operation.Type);
                return Task.FromResult(Result.Failure(ValueNotCompliant));
            }

            return Task.FromResult(Result.Success());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Operation: {operationId}] [OperationType: {operationType}] Failed to validate hex color value.",
                _operation.Id,
                _operation.Type);

            return Task.FromResult(Result.Failure(FailedToCreatePayload));
        }
    }

    public override Result<OperationValue> ProcessPayload(string payload)
    {
        try
        {
            if (Configuration.IsNotJson)
            {
                var isPayloadValid = payload.IsValidHexColor();

                if (isPayloadValid.IsFailure)
                {
                    logger.LogDebug("[Operation: {operationId}] [OperationType: {operationType}] Payload is not valid hex color.",
                        _operation.Id,
                        _operation.Type);

                    return Result<OperationValue>.Failure(ValueNotCompliant);
                }

                var notJsonValue = new OperationValue
                {
                    Operation = _operation,
                    Value = payload,
                    ProcessedAtUtc = DateTime.UtcNow,
                };

                return Result<OperationValue>.Success(notJsonValue);
            }

            var readResult = Configuration.FromJsonStringPayload(payload);

            if (readResult.IsFailure)
            {
                logger.LogDebug("[Operation: {operationId}] [OperationType: {operationType}] Payload is not valid hex color.",
                    _operation.Id,
                    _operation.Type);

                return Result<OperationValue>.Failure(ValueNotCompliant);
            }

            var data = readResult.Data;

            if (data.GetValueKind() != JsonValueKind.String)
            {
                logger.LogDebug("[Operation: {operationId}] [OperationType: {operationType}] Payload is not valid hex color.",
                    _operation.Id,
                    _operation.Type);

                return Result<OperationValue>.Failure(ValueNotCompliant);
            }

            var hexColor = data.GetValue<string>();

            var isValid = hexColor.IsValidHexColor();

            if (isValid.IsFailure)
            {
                logger.LogDebug("[Operation: {operationId}] [OperationType: {operationType}] Payload is not valid hex color.",
                    _operation.Id,
                    _operation.Type);

                return Result<OperationValue>.Failure(ValueNotCompliant);
            }


            var operationValue = new OperationValue
            {
                Operation = _operation,
                Value = hexColor,
                ProcessedAtUtc = DateTime.UtcNow,
            };

            return Result<OperationValue>.Success(operationValue);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Operation: {operationId}] [OperationType: {operationType}] Failed to process hex color payload.",
                _operation.Id,
                _operation.Type);

            return Result<OperationValue>.Failure(FailedToProcessPayload);
        }
    }

    public override Operation AsOperationEntity()
    {
        var preparedConfiguration = configuration.ToBaseConfiguration();
        _operation.Configuration = preparedConfiguration;
        return _operation;
    }

    public override async Task<Result> Validate(CancellationToken cancellationToken = default)
    {
        var validator = new HexColorOperationValidator();

        var validationResult = await validator.ValidateAsync(this, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToResult();

        return Result.Success();
    }
}
