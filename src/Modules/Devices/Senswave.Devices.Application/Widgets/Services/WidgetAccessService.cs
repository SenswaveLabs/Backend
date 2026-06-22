using Senswave.Devices.Domain.Devices.Services;
using Senswave.Devices.Domain.Widgets.Repositories;
using Senswave.Devices.Domain.Widgets.Services;

namespace Senswave.Devices.Application.Widgets.Services;

public class WidgetAccessService(
    IWidgetQueryRepository repository,
    IDeviceAccessService deviceAccessService,
    ILogger<WidgetAccessService> logger) : IWidgetAccessService
{
    #region Errors

    private readonly Error WidgetNotFound = Error.NotFound("WidgetNotFound", "Widget not found.");

    #endregion

    public async Task<Result> CanAct(Guid userId, Guid widgetId, CancellationToken cancellationToken)
    {
        var deviceId = await repository.GetDeviceIdByWidgetId(widgetId, cancellationToken);

        if (deviceId == default)
        {
            logger.LogWarning("[User: {userId}] [Widget: {widgetId}] Device for widget not found.", userId, widgetId);
            return Result.Failure(WidgetNotFound);
        }

        return await deviceAccessService.CanAct(userId, deviceId, cancellationToken);
    }

    public async Task<Result> CanDisplay(Guid userId, Guid widgetId, CancellationToken cancellationToken)
    {
        var deviceId = await repository.GetDeviceIdByWidgetId(widgetId, cancellationToken);

        if (deviceId == default)
        {
            logger.LogWarning("[User: {userId}] [Widget: {widgetId}] Device for widget not found.", userId, widgetId);
            return Result.Failure(WidgetNotFound);
        }

        return await deviceAccessService.CanDisplay(userId, deviceId, cancellationToken);
    }

    public Task<Result> CanDisplayDevice(Guid userId, Guid deviceId, CancellationToken cancellationToken)
        => deviceAccessService.CanDisplay(userId, deviceId, cancellationToken);

    public async Task<Result> CanManage(Guid userId, Guid widgetId, CancellationToken cancellationToken)
    {
        var deviceId = await repository.GetDeviceIdByWidgetId(widgetId, cancellationToken);

        if (deviceId == default)
        {
            logger.LogWarning("[User: {userId}] [Widget: {widgetId}] Device for widget not found.", userId, widgetId);
            return Result.Failure(WidgetNotFound);
        }

        return await deviceAccessService.CanManage(userId, deviceId, cancellationToken);
    }

    public Task<Result> CanManageDevice(Guid userId, Guid deviceId, CancellationToken cancellationToken)
        => deviceAccessService.CanManage(userId, deviceId, cancellationToken);

    public async Task<Result> IsOwner(Guid userId, Guid widgetId, CancellationToken cancellationToken)
    {
        var deviceId = await repository.GetDeviceIdByWidgetId(widgetId, cancellationToken);

        if (deviceId == default)
        {
            logger.LogWarning("[User: {userId}] [Widget: {widgetId}] Device for widget not found.", userId, widgetId);
            return Result.Failure(WidgetNotFound);
        }

        return await deviceAccessService.IsOwner(userId, deviceId, cancellationToken);
    }
}
