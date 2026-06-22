using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Operations.Enums;
using Senswave.Devices.Domain.Operations.Models;
using Senswave.Devices.Domain.Operations.Types.Base;
using Senswave.Devices.Domain.Operations.ValueObjects;

namespace Senswave.Devices.Domain.Operations.Types;

public interface IOperation
{
    /// <summary>
    /// Returns id for operation.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Returns device id for operation.
    /// </summary>
    Guid DeviceId { get; }

    /// <summary>
    /// Returns reference for data source observation.
    /// </summary>
    Guid? DataReferenceId { get; }

    /// <summary>
    /// Returns Name of operation.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Returns type of operation.
    /// </summary>
    OperationType Type { get; }

    /// <summary>
    /// Returns configuration of Operation.
    /// </summary>
    BaseOperationConfiguration Configuration { get; }

    /// <summary>
    /// Function creates payload that will be send to device with provided value.
    /// </summary>
    /// <param name="value">Value to Send as a jsonNode </param>
    /// <returns></returns>
    Result<OperationAssembledModel> CreatePayload(JsonValue value);

    /// <summary>
    /// Function prcesses payload received from device.
    /// </summary>
    /// <param name="payload"></param>
    /// <returns></returns>
    Result<OperationValue> ProcessPayload(string payload);

    /// <summary>
    /// Validates opertaion.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result> Validate(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if json node value type is copatible with operation.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    Task<Result> IsValueCompliant(JsonValue value);

    /// <summary>
    /// Converts data to operation entity.
    /// </summary>
    /// <returns></returns>
    Operation AsOperationEntity();

    /// <summary>
    /// Returns the latest operation value ordered by processed time.
    /// </summary>
    Task<Result<OperationValue>> GetCurrentValue(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get operation values to past historic date.
    /// </summary>
    /// <param name="toUtc"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result<List<OperationValue>>> GetValuesToPastTime(int maxRows, CancellationToken cancellationToken = default);
}