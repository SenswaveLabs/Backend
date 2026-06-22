using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Operations.Enums;
using Senswave.Devices.Domain.Operations.Models;
using Senswave.Devices.Domain.Operations.ValueObjects;

namespace Senswave.Devices.Domain.Operations.Types.Base;

public abstract class BaseOperation(
    Operation operation,
    BaseOperationConfiguration configuration) : IOperation
{
    protected static readonly Error InvalidDataType = Error.Failure("InvalidDataType", "Received value is not matching operation type.");

    protected static readonly Error FailedToCreatePayload = Error.Failure("FailedToCreatePayload", "Failed to create payloadto send.");

    protected static readonly Error FailedToProcessPayload = Error.Failure("FailedToProcessPayload", "Failed to process received payload.");

    protected static readonly Error ValueNotCompliant = Error.Validation("ValueNotCompliant", "Received value is not matching operation type.");

    protected readonly Operation _operation = operation;

    public Guid Id => _operation.Id;

    public Guid DeviceId => _operation.DeviceId;

    public Guid? DataReferenceId => _operation.DataReferenceId;

    public string Name => _operation.Name;

    public OperationType Type => _operation.Type;

    public BaseOperationConfiguration Configuration => configuration;

    public abstract Result<OperationAssembledModel> CreatePayload(JsonValue value);

    public abstract Result<OperationValue> ProcessPayload(string payload);

    public abstract Task<Result> Validate(CancellationToken cancellationToken = default);

    public abstract Task<Result> IsValueCompliant(JsonValue value);

    public abstract Operation AsOperationEntity();

    public virtual Task<Result<OperationValue>> GetCurrentValue(CancellationToken cancellationToken = default)
    {
        if (_operation.Values.Count == 0)
            return Task.FromResult(Result<OperationValue>.Failure(Error.Failure("OperationHasNoValues", "Operation has no operation values yet.")));

        var latest = _operation.Values
            .OrderByDescending(x => x.ProcessedAtUtc)
            .First();

        return Task.FromResult(Result<OperationValue>.Success(latest));
    }

    public virtual Task<Result<List<OperationValue>>> GetValuesToPastTime(int maxRows, CancellationToken cancellationToken = default)
    {
        var valuesInRange = _operation.Values
            .OrderByDescending(x => x.ProcessedAtUtc)
            .Take(maxRows)
            .ToList();

        return Task.FromResult(Result<List<OperationValue>>.Success(valuesInRange));
    }
}
