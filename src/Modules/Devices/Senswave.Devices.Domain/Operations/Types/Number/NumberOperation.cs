using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Operations.Extensions;
using Senswave.Devices.Domain.Operations.Models;
using Senswave.Devices.Domain.Operations.Types.Base;
using Senswave.Devices.Domain.Operations.Types.Characteristics.Range;
using Senswave.Devices.Domain.Operations.Types.Number.DecimalSeparator;
using Senswave.Devices.Domain.Operations.ValueObjects;
using System.Globalization;
using System.Text.Json;

namespace Senswave.Devices.Domain.Operations.Types.Number;

public sealed class NumberOperation(
    Operation operation,
    NumberOperationConfiguration configuration,
    ILogger<IOperation> logger) : BaseOperation(operation, configuration), IRangeCharacteristic<double>
{
    private static readonly NumberFormatInfo _commaFormatInfo = new() { NumberDecimalSeparator = "," };

    #region Errors

    private static readonly Error NumberIsNotInRange = Error.Failure("NumberIsNotInRange", "Number is not matching provided operation range.");
    private static readonly Error PayloadIsNotValidNumber = Error.Failure("PayloadIsNotValidNumber", "Received payload is not valid number.");

    #endregion

    public new NumberOperationConfiguration Configuration => configuration;

    public override Result<OperationAssembledModel> CreatePayload(JsonValue jsonValue)
    {
        try
        {
            var valueKind = jsonValue.GetValueKind();

            if (valueKind != JsonValueKind.Number)
                return Result<OperationAssembledModel>.Failure(InvalidDataType);

            var value = jsonValue.GetValue<double>();

            if (value < Configuration.Min || value > Configuration.Max)
                return Result<OperationAssembledModel>.Failure(NumberIsNotInRange);

            var data = new OperationValue()
            {
                Operation = _operation,
                Value = value,
                ProcessedAtUtc = DateTime.UtcNow,
            };

            if (!Configuration.IsJson)
            {
                var doublePayload = ToStringDouble(value);

                var assembledDot = new OperationAssembledModel
                {
                    Payload = doublePayload,
                    Value = data
                };

                return Result<OperationAssembledModel>.Success(assembledDot);
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
            logger.LogError(ex, "[Operation: {operationId}] [OperationType: {operationType}] Failed to create number payload.",
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
                bool result = false;
                double parsedValue = 0;

                if (configuration.DecimalSeparator.ToDecimalSeparator() == DecimalSeparatorType.Comma)
                {
                    if (payload.Contains('.'))
                        return Result<OperationValue>.Failure(PayloadIsNotValidNumber);

                    result = double.TryParse(payload, _commaFormatInfo, out parsedValue);
                }
                else
                {
                    if (payload.Contains(','))
                        return Result<OperationValue>.Failure(PayloadIsNotValidNumber);

                    result = double.TryParse(payload, CultureInfo.InvariantCulture, out parsedValue);
                }

                if (!result)
                    return Result<OperationValue>.Failure(PayloadIsNotValidNumber);

                if (parsedValue < Configuration.Min || parsedValue > Configuration.Max)
                    return Result<OperationValue>.Failure(NumberIsNotInRange);

                var operationValue = new OperationValue
                {
                    Operation = _operation,
                    Value = parsedValue,
                    ProcessedAtUtc = DateTime.UtcNow
                };

                return Result<OperationValue>.Success(operationValue);
            }

            var readResult = configuration.FromJsonStringPayload(payload);

            if (readResult.IsFailure)
                return Result<OperationValue>.Failure(readResult.Errors);

            var kind = readResult.Data.GetValueKind();

            if (JsonValueKind.Number != kind)
                return Result<OperationValue>.Failure(PayloadIsNotValidNumber);

            var value = readResult.Data.GetValue<double>();

            if (value < Configuration.Min || value > Configuration.Max)
                return Result<OperationValue>.Failure(NumberIsNotInRange);

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
            logger.LogError(ex, "[Operation: {operationId}] [OperationType: {operationType}] Failed to process number payload.",
                _operation.Id,
                _operation.Type);

            return Result<OperationValue>.Failure(FailedToProcessPayload);
        }
    }

    public override async Task<Result> Validate(CancellationToken cancellationToken = default)
    {
        var validator = new NumberOperationValidator();

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

            var number = value.GetValue<double>();

            if (number < Configuration.Min || number > Configuration.Max)
                return Task.FromResult(Result.Failure(Error.Validation("ValueNotInRange")));

            return Task.FromResult(Result.Success());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Operation: {operationId}] [OperationType: {operationType}] Failed to check if value is compliant with number operation.",
                _operation.Id,
                _operation.Type);

            return Task.FromResult(Result.Failure(Error.Failure("ErroWhenCheckingCompliance", "An error occurred while checking value compliance.")));
        }
    }

    public override Operation AsOperationEntity()
    {
        var preparedConfiguration = configuration.ToBaseConfiguration();
        preparedConfiguration["min"] = Configuration.Min;
        preparedConfiguration["max"] = Configuration.Max;
        preparedConfiguration["decimalSeparator"] = Configuration.DecimalSeparator;
        _operation.Configuration = preparedConfiguration;
        return _operation;
    }

    #region IRangeCharacteristic

    public Task<Result> ValidateStep(object stepObject)
    {
        if (Configuration.Max == double.MaxValue && Configuration.Min == double.MinValue)
            return Task.FromResult(Result.Failure(RangeCharacteristicErrors.InvalidCharacteristicRange));

        double step;

        if (stepObject is int i)
            step = i;
        else if (stepObject is double d)
            step = d;
        else
            return Task.FromResult(Result.Failure(RangeCharacteristicErrors.InvalidCharacteristicStepType));

        if (step <= 0.0001)
            return Task.FromResult(Result.Failure(RangeCharacteristicErrors.CharacteristicStepIsTooSmall));

        var sum = Configuration.Min + step;

        if (sum > Configuration.Max)
            return Task.FromResult(Result.Failure(RangeCharacteristicErrors.CharacteristicStepIsTooBig));

        return Task.FromResult(Result.Success());
    }

    public Task<Result<JsonObject>> GetDisplayRange() => Task.FromResult(Result<JsonObject>.Success(new JsonObject
    {
        ["min"] = Configuration.Min,
        ["max"] = Configuration.Max,
    }));

    #endregion

    #region Privates

    private string ToStringDouble(double value)
    {
        if (configuration.DecimalSeparator.ToDecimalSeparator() == DecimalSeparatorType.Comma)
            return value.ToString(_commaFormatInfo);

        return value.ToString(CultureInfo.InvariantCulture);
    }

    #endregion
}
