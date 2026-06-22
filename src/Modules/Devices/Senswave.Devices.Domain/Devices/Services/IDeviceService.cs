using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Devices.Models;

namespace Senswave.Devices.Domain.Devices.Services;

public interface IDeviceService
{
    /// <summary>
    /// Sends a presence update event associated with the specified operation identifier.
    /// </summary>
    /// <remarks>This method is typically used to notify other components of a change in presence status.
    /// Ensure that the operationId is valid and corresponds to an active session.</remarks>
    /// <param name="operationId">The unique identifier for the operation that this presence update event corresponds to. Must be a valid and
    /// active operation identifier.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a value indicating the outcome of
    /// the presence update event.</returns>
    Task DevicePresenceEvent(List<Guid> operationIds, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if device is connected with service linked by operationId.
    /// </summary>
    /// <param name="operationId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result> IsDevicePresentByOperationId(Guid operationId, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if device is connected with service linked by widgetId.
    /// </summary>
    /// <param name="widgetId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result> IsDevicePresentByWidgetId(Guid widgetId, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if device is connected with service.
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result> IsDevicePresent(Guid deviceId, CancellationToken cancellationToken);

    /// <summary>
    /// Returns displayable model for frontend.
    /// </summary>
    /// <param name="device"></param>
    /// <returns></returns>
    Task<Result<DisplayDeviceModel>> Interpret(Guid deviceId, CancellationToken cancellationToken);

    /// <summary>
    /// Returns displayable model for frontend.
    /// </summary>
    /// <param name="device"></param>
    /// <returns></returns>
    Task<Result<DisplayDeviceModel>> Interpret(Device device);

    /// <summary>
    /// Assembles value to send based on device configuration. This is used when sending command to device.
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="value"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result<DeviceTileMessageModel>> PreprocessValueForTileMessage(Guid deviceId, JsonValue value, CancellationToken cancellationToken);
}
