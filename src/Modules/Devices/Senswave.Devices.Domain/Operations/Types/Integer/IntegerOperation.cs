using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Operations.Extensions;
using Senswave.Devices.Domain.Operations.Models;
using Senswave.Devices.Domain.Operations.Types.Base;
using Senswave.Devices.Domain.Operations.Types.Characteristics.Range;
using Senswave.Devices.Domain.Operations.ValueObjects;
using System.Text.Json;

namespace Senswave.Devices.Domain.Operations.Types.Integer;

public sealed class IntegerOperation(
    Operation operation,
    IntegerOperationConfiguration configuration,
    ILogger<IOperation> logger) : BaseOperation(operation, configuration), IRangeCharacteristic<int>
{
    private static readonly Error IntegerIsNotInRange = Error.Failure("IntegerIsNotInRange", "Integer is not matching provided operation range.");

    private static readonly Error PayloadIsNotValidInteger = Error.Failure("PayloadIsNotValidInteger", "Received payload is not valid integer.");

    public new IntegerOperationConfiguration Configuration => configuration;

    public override Result<OperationAssembledModel> CreatePayload(JsonValue jsonValue)
    {
        try
        {
            var valueKind = jsonValue.GetValueKind();

            if (valueKind != JsonValueKind.Number)
                return Result<OperationAssembledModel>.Failure(InvalidDataType);

            var value = jsonValue.GetValue<int>();

            if (value < Configuration.Min || value > Configuration.Max)
                return Result<OperationAssembledModel>.Failure(IntegerIsNotInRange);

            var data = new OperationValue()
            {
                Operation = _operation,
                Value = value,
                ProcessedAtUtc = DateTime.UtcNow,
            };

            if (!Configuration.IsJson)
            {
                var assembled = new OperationAssembledModel
                {
                    Payload = value.ToString(),
                    Value = data
                };

                return Result<OperationAssembledModel>.Success(assembled);
            }

            var payload = configuration.ToJsonStringPayload(value);

            var assembledJson = new OperationAssembledModel
            {
                Payload = payload,
                Value = data
            };

            return Result<OperationAssembledModel>.Success(assembledJson);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Operation: {operationId}] [OperationType: {operationType}] Failed to create integer payload.",
                _operation.Id,
                _operation.Type);

            return Result<OperationAssembledModel>.Failure(FailedToCreatePayload);
        }
    }

    public override Result<OperationValue> ProcessPayload(string payload)
    {
        try
        {
            if (!Configuration.IsJson)
            {
                var result = int.TryParse(payload, out int parsedValue);

                if (!result)
                    return Result<OperationValue>.Failure(PayloadIsNotValidInteger);

                if (parsedValue < Configuration.Min || parsedValue > Configuration.Max)
                    return Result<OperationValue>.Failure(IntegerIsNotInRange);

                var operationValue = new OperationValue
                {
                    Operation = _operation,
                    Value = parsedValue,
                    ProcessedAtUtc = DateTime.UtcNow
                };

                return Result<OperationValue>.Success(operationValue);
            }

            var readResult = Configuration.FromJsonStringPayload(payload);

            if (readResult.IsFailure)
                return Result<OperationValue>.Failure(readResult.Errors);

            var kind = readResult.Data.GetValueKind();

            if (JsonValueKind.Number != kind)
                return Result<OperationValue>.Failure(PayloadIsNotValidInteger);

            var value = readResult.Data.GetValue<int>();

            if (value < Configuration.Min || value > Configuration.Max)
                return Result<OperationValue>.Failure(IntegerIsNotInRange);

            var operationValueFromJson = new OperationValue
            {
                Operation = _operation,
                Value = value,
                ProcessedAtUtc = DateTime.UtcNow
            };

            return Result<OperationValue>.Success(operationValueFromJson);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Operation: {operationId}] [OperationType: {operationType}] Failed to process integer payload.",
                _operation.Id,
                _operation.Type);

            return Result<OperationValue>.Failure(FailedToProcessPayload);
        }
    }

    public override async Task<Result> Validate(CancellationToken cancellationToken = default)
    {
        var validator = new IntegerOperationValidator();

        var validationResult = await validator.ValidateAsync(this, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToResult();

        return Result.Success();
    }

    public override Task<Result> IsValueCompliant(JsonValue value)
    {
        try
        {
            if (value.GetValueKind() != JsonValueKind.Number)
                return Task.FromResult(Result.Failure(ValueNotCompliant));

            var parsedValue = value.GetValue<int>();

            if (parsedValue < Configuration.Min || parsedValue > Configuration.Max)
                return Task.FromResult(Result.Failure(Error.Validation("ValueNotInRange")));

            return Task.FromResult(Result.Success());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Operation: {operationId}] [OperationType: {operationType}] Failed to check if value is compliant with integer operation.",
                _operation.Id,
                _operation.Type);

            return Task.FromResult(Result.Failure(Error.Failure("ErrorWhenCheckingCompliance", "An error occurred while checking value compliance.")));
        }
    }

    public override Operation AsOperationEntity()
    {
        var preparedConfiguration = configuration.ToBaseConfiguration();
        preparedConfiguration["min"] = Configuration.Min;
        preparedConfiguration["max"] = Configuration.Max;
        _operation.Configuration = preparedConfiguration;
        return _operation;
    }

    #region IRangeCharacteristic

    public Task<Result> ValidateStep(object stepObject)
    {
        if (Configuration.Min == int.MinValue && Configuration.Max == int.MaxValue)
            return Task.FromResult(Result.Failure(RangeCharacteristicErrors.InvalidCharacteristicRange));

        if (stepObject is not int)
            return Task.FromResult(Result.Failure(RangeCharacteristicErrors.InvalidCharacteristicStepType));

        var step = (int)stepObject;

        var sum = Configuration.Min + step;

        if (sum >= Configuration.Max)
            return Task.FromResult(Result.Failure(RangeCharacteristicErrors.CharacteristicStepIsTooBig));

        if (sum <= Configuration.Min)
            return Task.FromResult(Result.Failure(RangeCharacteristicErrors.CharacteristicStepIsTooSmall));

        return Task.FromResult(Result.Success());
    }

    public Task<Result<JsonObject>> GetDisplayRange() => Task.FromResult(Result<JsonObject>.Success(new JsonObject
    {
        ["min"] = Configuration.Min,
        ["max"] = Configuration.Max,
    }));

    #endregion
}
