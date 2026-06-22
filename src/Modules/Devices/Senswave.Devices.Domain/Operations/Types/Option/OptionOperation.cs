using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Operations.Extensions;
using Senswave.Devices.Domain.Operations.Models;
using Senswave.Devices.Domain.Operations.Types.Base;
using Senswave.Devices.Domain.Operations.Types.Option.Models;
using Senswave.Devices.Domain.Operations.ValueObjects;
using System.Globalization;
using System.Text.Json;

namespace Senswave.Devices.Domain.Operations.Types.Option;

public class OptionOperation(
    Operation operation,
    OptionOperationConfiguration configuration,
    ILogger<IOperation> logger) : BaseOperation(operation, configuration)
{
    public new OptionOperationConfiguration Configuration => configuration;

    /// <summary>
    /// Creates payload for the operation based on the provided name of operation.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public override Result<OperationAssembledModel> CreatePayload(JsonValue value)
    {
        try
        {
            var matchedByName = MatchByName(value);

            if (matchedByName.IsFailure)
            {
                logger.LogWarning("[Operation: {operationId}] [OperationType: {operationType}] Received payload value is not in options list.",
                     _operation.Id, _operation.Type);

                return Result<OperationAssembledModel>.Failure(matchedByName.Errors);
            }

            var data = new OperationValue
            {
                Operation = _operation,
                Value = matchedByName.Data.Name,
                ProcessedAtUtc = DateTime.UtcNow
            };

            if (!Configuration.IsJson)
            {
                var assembled = new OperationAssembledModel
                {
                    Payload = matchedByName.Data.Value.ToString(),
                    Value = data
                };
                return Result<OperationAssembledModel>.Success(assembled);
            }

            var objectValue = FromOptionValue(matchedByName.Data);

            var payload = configuration.ToJsonStringPayload(objectValue);
            var assembledJson = new OperationAssembledModel
            {
                Payload = payload,
                Value = data
            };

            return Result<OperationAssembledModel>.Success(assembledJson);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Operation: {operationId}] [OperationType: {operationType}] Failed to create option payload.",
                _operation.Id,
                _operation.Type);

            return Result<OperationAssembledModel>.Failure(FailedToProcessPayload);
        }
    }

    /// <summary>
    /// Operation should be matched by option name, not value.
    /// </summary>
    /// <param name="value">String option name</param>
    /// <returns></returns>
    public override Task<Result> IsValueCompliant(JsonValue value)
    {
        try
        {
            var matchedByValue = MatchByName(value);

            if (matchedByValue.IsSuccess)
                return Task.FromResult(Result.Success());

            return Task.FromResult(Result.Failure(ValueNotCompliant));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Operation: {operationId}] [OperationType: {operationType}] Failed to validate option value.",
                _operation.Id,
                _operation.Type);

            return Task.FromResult(Result.Failure(FailedToProcessPayload));
        }
    }

    public override Result<OperationValue> ProcessPayload(string payload)
    {
        try
        {
            if (!Configuration.IsJson)
            {
                foreach (var option in Configuration.Options)
                {
                    var optionKind = option.Value.GetValueKind();

                    if (optionKind == JsonValueKind.Number)
                    {
                        var normalizedPayload = payload.Replace(',', '.');

                        if (double.TryParse(normalizedPayload, NumberStyles.Any, CultureInfo.InvariantCulture, out double doubleValue) &&
                            option.Value.GetValue<double>() == doubleValue)
                        {
                            var operationValueData = new OperationValue
                            {
                                Operation = _operation,
                                Value = option.Name,
                                ProcessedAtUtc = DateTime.UtcNow
                            };

                            return Result<OperationValue>.Success(operationValueData);
                        }
                    }

                    if (optionKind == JsonValueKind.String &&
                        option.Value.GetValue<string>() == payload)
                    {
                        var operationValueNumber = new OperationValue
                        {
                            Operation = _operation,
                            Value = option.Name,
                            ProcessedAtUtc = DateTime.UtcNow
                        };

                        return Result<OperationValue>.Success(operationValueNumber);
                    }

                    if ((optionKind == JsonValueKind.True || optionKind == JsonValueKind.False) &&
                        bool.TryParse(payload, out bool boolValue) &&
                        option.Value.GetValue<bool>() == boolValue)
                    {
                        var operationValueData = new OperationValue
                        {
                            Operation = _operation,
                            Value = option.Name,
                            ProcessedAtUtc = DateTime.UtcNow
                        };

                        return Result<OperationValue>.Success(operationValueData);
                    }
                }

                logger.LogWarning("[Operation: {operationId}] [OperationType: {operationType}] Received payload value is not in options values list.",
                    _operation.Id,
                    _operation.Type);

                return Result<OperationValue>.Failure(OptionOperationErrors.FailedToMatchOptionByValue);
            }

            var readResult = Configuration.FromJsonStringPayload(payload);

            if (readResult.IsFailure)
                return Result<OperationValue>.Failure(readResult.Errors);

            var matchedByValue = MatchByValue(readResult.Data);

            if (matchedByValue.IsFailure)
            {
                logger.LogWarning("[Operation: {operationId}] [OperationType: {operationType}] [Json] Received payload value is not in options values list.",
                    _operation.Id,
                    _operation.Type);

                return Result<OperationValue>.Failure(matchedByValue.Errors);
            }

            var operationValueFromJson = new OperationValue
            {
                Operation = _operation,
                Value = matchedByValue.Data.Name,
                ProcessedAtUtc = DateTime.UtcNow
            };

            return Result<OperationValue>.Success(operationValueFromJson);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Operation: {operationId}] [OperationType: {operationType}] Failed to process option payload.",
                _operation.Id,
                _operation.Type);
            return Result<OperationValue>.Failure(FailedToProcessPayload);
        }
    }

    public override Operation AsOperationEntity()
    {
        var preparedConfiguration = configuration.ToBaseConfiguration();

        var options = JsonSerializer.SerializeToNode(Configuration.Options);

        preparedConfiguration["options"] = options;

        _operation.Configuration = preparedConfiguration;
        return _operation;
    }

    public override async Task<Result> Validate(CancellationToken cancellationToken = default)
    {
        var validator = new OptionOperationValidator();

        var validationResult = await validator.ValidateAsync(this, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToResult();

        return Result.Success();
    }

    #region Privates

    private static object FromOptionValue(OptionInfo option)
    {
        if (option.Value.GetValueKind() == JsonValueKind.Number)
            return option.Value.GetValue<double>();

        if (option.Value.GetValueKind() == JsonValueKind.String)
            return option.Value.GetValue<string>();

        if (option.Value.GetValueKind() == JsonValueKind.True || option.Value.GetValueKind() == JsonValueKind.False)
            return option.Value.GetValue<bool>();

        return "";
    }

    private Result<OptionInfo> MatchByValue(JsonNode value)
    {
        var valueKind = value.GetValueKind();

        if (valueKind == JsonValueKind.Undefined ||
            valueKind == JsonValueKind.Null ||
            valueKind == JsonValueKind.Array)
            return Result<OptionInfo>.Failure(ValueNotCompliant);

        foreach (var option in Configuration.Options)
        {
            var optionKind = option.Value.GetValueKind();

            if (optionKind != valueKind)
                continue;

            if (optionKind == JsonValueKind.Number && option.Value.GetValue<double>() == value.GetValue<double>())
                return Result<OptionInfo>.Success(option);

            if (optionKind == JsonValueKind.String && option.Value.GetValue<string>() == value.GetValue<string>())
                return Result<OptionInfo>.Success(option);

            if ((optionKind == JsonValueKind.True || optionKind == JsonValueKind.False) && option.Value.GetValue<bool>() == value.GetValue<bool>())
                return Result<OptionInfo>.Success(option);
        }

        return Result<OptionInfo>.Failure(OptionOperationErrors.FailedToMatchOptionByValue);
    }

    private Result<OptionInfo> MatchByName(JsonNode value)
    {
        var valueKind = value.GetValueKind();

        if (valueKind != JsonValueKind.String)
            return Result<OptionInfo>.Failure(ValueNotCompliant);

        var optionName = value.GetValue<string>();

        foreach (var option in Configuration.Options)
        {
            if (optionName == option.Name)
                return Result<OptionInfo>.Success(option);
        }

        return Result<OptionInfo>.Failure(OptionOperationErrors.FailedToCreatePayload);
    }

    #endregion
}
