using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Operations.Extensions;
using Senswave.Devices.Domain.Operations.Models;
using Senswave.Devices.Domain.Operations.Types.Base;
using Senswave.Devices.Domain.Operations.ValueObjects;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Senswave.Devices.Domain.Operations.Types.Text;

public sealed class TextOperation(
    Operation operation,
    TextOperationConfiguration configuration,
    ILogger<IOperation> logger) : BaseOperation(operation, configuration)
{
    public const int MaxInputLength = 2048;

    public new TextOperationConfiguration Configuration => configuration;

    public override Result<OperationAssembledModel> CreatePayload(JsonValue jsonValue)
    {
        try
        {
            var valueKind = jsonValue.GetValueKind();

            if (valueKind != JsonValueKind.String)
                return Result<OperationAssembledModel>.Failure(InvalidDataType);

            var value = jsonValue.AsValue().ToString();

            var validationResult = BaseValidateValue(value);

            if (!validationResult)
                return Result<OperationAssembledModel>.Failure(validationResult.Errors);

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
            logger.LogError(ex, "[Operation: {operationId}] [OperationType: {operationType}] Failed to create text payload.",
                _operation.Id,
                _operation.Type);

            return Result<OperationAssembledModel>.Failure(FailedToCreatePayload);
        }
        ;
    }

    public override Result<OperationValue> ProcessPayload(string payload)
    {
        try
        {
            if (!Configuration.IsJson)
            {
                var baseValidationResult = BaseValidateValue(payload);

                if (!baseValidationResult)
                    return Result<OperationValue>.Failure(baseValidationResult.Errors);

                var operationValue = new OperationValue
                {
                    Operation = _operation,
                    InternalValue = new()
                    {
                        ["value"] = payload
                    },
                    ProcessedAtUtc = DateTime.UtcNow
                };

                return Result<OperationValue>.Success(operationValue);
            }

            var readResult = Configuration.FromJsonStringPayload(payload);

            if (readResult.IsFailure)
                return Result<OperationValue>.Failure(readResult.Errors);

            if (readResult.Data.GetValueKind() != JsonValueKind.String)
                return Result<OperationValue>.Failure(Error.Failure("JsonFieldIsNotValidString", "Processed value is not valid for text operation."));

            var value = readResult.Data.ToString();

            var validationResult = BaseValidateValue(value);

            if (!validationResult)
                return Result<OperationValue>.Failure(validationResult.Errors);

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
            logger.LogError(ex, "[Operation: {operationId}] [OperationType: {operationType}] Failed to process boolean payload.",
                _operation.Id,
                _operation.Type);

            return Result<OperationValue>.Failure(FailedToProcessPayload);
        }
    }

    public override Operation AsOperationEntity()
    {
        _operation.Configuration = configuration.ToBaseConfiguration();
        return _operation;
    }

    public override Task<Result> IsValueCompliant(JsonValue value)
    {
        if (value.GetValueKind() != JsonValueKind.String)
            return Task.FromResult(Result.Failure(ValueNotCompliant));

        var stringValue = value.GetValue<string>();

        var validationResult = BaseValidateValue(stringValue);

        if (!BaseValidateValue(stringValue))
            return Task.FromResult(Result.Failure(validationResult.Errors));

        return Task.FromResult(Result.Success());
    }

    public override async Task<Result> Validate(CancellationToken cancellationToken = default)
    {
        var validator = new TextOperationValidator();

        var validationResult = await validator.ValidateAsync(this, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToResult();

        return Result.Success();
    }

    #region Private

    private static Result BaseValidateValue(string value)
    {
        const string pattern = @"^[ a-zA-Z0-9#+-_().,]*$";

        if (value.Length > MaxInputLength)
            return Result<OperationAssembledModel>.Failure(Error.Validation("TooLongInput", "Provided value to send is too long."));

        if (!Regex.IsMatch(value, pattern))
            return Result<OperationAssembledModel>.Failure(Error.Validation("InvalidCharactersInString", "Provided value to send has invalid characters."));

        return Result.Success();
    }

    #endregion
}
