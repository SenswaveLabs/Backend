using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Operations.Extensions;
using Senswave.Devices.Domain.Operations.Models;
using Senswave.Devices.Domain.Operations.Types.Base;
using Senswave.Devices.Domain.Operations.ValueObjects;
using System.Text.Json;

namespace Senswave.Devices.Domain.Operations.Types.Boolean;

public sealed class BooleanOperation(
    Operation operation,
    BooleanOperationConfiguration configuration,
    ILogger<IOperation> logger) : BaseOperation(operation, configuration)
{
    public new BooleanOperationConfiguration Configuration => configuration;

    public override Result<OperationAssembledModel> CreatePayload(JsonValue jsonValue)
    {
        try
        {
            var valueKind = jsonValue.GetValueKind();

            if (valueKind != JsonValueKind.True && valueKind != JsonValueKind.False)
                return Result<OperationAssembledModel>.Failure(InvalidDataType);

            var value = jsonValue.GetValue<bool>();
            var mapped = value ? Configuration.EffectiveTrueValue : Configuration.EffectiveFalseValue;

            var data = new OperationValue
            {
                Value = value,
                ProcessedAtUtc = DateTime.UtcNow,
            };

            var payload = Configuration.IsJson
                ? BuildJsonPayload(mapped)
                : ToRawString(mapped);

            return Result<OperationAssembledModel>.Success(new OperationAssembledModel { Payload = payload, Value = data });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Operation: {operationId}] [OperationType: {operationType}] Failed to create boolean payload.",
                _operation.Id,
                _operation.Type);

            return Result<OperationAssembledModel>.Failure(FailedToCreatePayload);
        }
    }

    public override Result<OperationValue> ProcessPayload(string payload)
    {
        try
        {
            Result<bool> resolved;

            if (!Configuration.IsJson)
            {
                var trueStr = ToRawString(Configuration.EffectiveTrueValue);
                var falseStr = ToRawString(Configuration.EffectiveFalseValue);
                resolved = Resolve(payload, trueStr, falseStr);
            }
            else
            {
                var readResult = Configuration.FromJsonStringPayload(payload);
                if (readResult.IsFailure)
                    return Result<OperationValue>.Failure(readResult.Errors);

                var trueStr = (Configuration.EffectiveTrueValue).ToJsonString();
                var falseStr = (Configuration.EffectiveFalseValue).ToJsonString();
                resolved = Resolve(readResult.Data.ToJsonString(), trueStr, falseStr);
            }

            if (resolved.IsFailure)
                return Result<OperationValue>.Failure(resolved.Errors);

            var operationValue = new OperationValue
            {
                Operation = _operation,
                InternalValue = new() { ["value"] = resolved.Data },
                ProcessedAtUtc = DateTime.UtcNow
            };

            return Result<OperationValue>.Success(operationValue);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Operation: {operationId}] [OperationType: {operationType}] Failed to process boolean payload.",
                _operation.Id,
                _operation.Type);

            return Result<OperationValue>.Failure(FailedToProcessPayload);
        }
    }

    public override async Task<Result> Validate(CancellationToken cancellationToken = default)
    {
        var validator = new BooleanOperationValidator();

        var validationResult = await validator.ValidateAsync(this, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToResult();

        return Result.Success();
    }

    public override Task<Result> IsValueCompliant(JsonValue value)
    {
        try
        {
            var valueKind = value.GetValueKind();

            if (valueKind != JsonValueKind.True && valueKind != JsonValueKind.False)
                return Task.FromResult(Result.Failure(ValueNotCompliant));

            return Task.FromResult(Result.Success());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Operation: {operationId}] [OperationType: {operationType}] Failed to check if value is compliant with boolean operation.",
                _operation.Id,
                _operation.Type);

            return Task.FromResult(Result.Failure(Error.Failure("ErroWhenCheckingCompliance", "An error occurred while checking value compliance.")));
        }
    }

    public override Operation AsOperationEntity()
    {
        var preparedConfiguration = configuration.ToBaseConfiguration();

        preparedConfiguration["trueValue"] = configuration.EffectiveTrueValue;
        preparedConfiguration["falseValue"] = configuration.EffectiveFalseValue;

        _operation.Configuration = preparedConfiguration;

        return _operation;
    }

    private static Result<bool> Resolve(string payload, string trueComparable, string falseComparable)
    {
        if (payload == trueComparable)
            return Result<bool>.Success(true);
        if (payload == falseComparable)
            return Result<bool>.Success(false);

        return Result<bool>.Failure(Error.Failure("PayloadIsNotValidBoolean", "Processed value is not valid type."));
    }

    private static string ToRawString(JsonValue jv)
    {
        return jv.GetValueKind() switch
        {
            JsonValueKind.String => jv.GetValue<string>(),
            _ => jv.ToString()
        };
    }

    private string BuildJsonPayload(JsonValue mappedValue)
    {
        JsonObject? payload = null;
        var names = configuration.JsonNames;
        for (int i = names.Length - 1; i >= 0; i--)
        {
            payload = payload is null
                ? new JsonObject { [names[i]] = mappedValue.DeepClone() }
                : new JsonObject { [names[i]] = payload };
        }
        return payload!.ToJsonString();
    }
}
