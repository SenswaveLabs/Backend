using Senswave.Devices.Domain.Dashboards.Repositories;
using Senswave.Devices.Domain.Dashboards.Services;
using Senswave.Devices.Domain.Devices.Services;

namespace Senswave.Devices.Application.Dashboards.Services;

public class DashboardAccessService(
    IDashboardsQueryRespository respository,
    IDeviceAccessService deviceAccessService,
    ILogger<DashboardAccessService> logger) : IDashboardAccessService
{
    #region Errors

    private readonly Error DashboardNotFound = Error.NotFound("DashboardNotFound", "Dashboard not found.");

    #endregion

    public async Task<Result> CanDisplay(Guid userId, Guid dashboardId, CancellationToken cancellationToken)
    {
        var deviceId = await respository.GetDeviceIdByDashboardId(dashboardId, cancellationToken);

        if (deviceId == default)
        {
            logger.LogWarning("[User: {UserId}] attempted to access non-existing dashboard: {DashboardId}.", userId, dashboardId);
            return Result.Failure(DashboardNotFound);
        }

        return await deviceAccessService.CanDisplay(userId, deviceId, cancellationToken);
    }

    public Task<Result> CanDisplayDevice(Guid userId, Guid deviceId, CancellationToken cancellationToken)
        => deviceAccessService.CanDisplay(userId, deviceId, cancellationToken);

    public async Task<Result> CanManage(Guid userId, Guid dashboardId, CancellationToken cancellationToken)
    {
        var deviceId = await respository.GetDeviceIdByDashboardId(dashboardId, cancellationToken);

        if (deviceId == default)
        {
            logger.LogWarning("[User: {UserId}] attempted to manage non-existing dashboard: {DashboardId}.", userId, dashboardId);
            return Result.Failure(DashboardNotFound);
        }

        return await deviceAccessService.CanManage(userId, deviceId, cancellationToken);
    }

    public Task<Result> CanManageDevice(Guid userId, Guid deviceId, CancellationToken cancellationToken)
        => deviceAccessService.CanManage(userId, deviceId, cancellationToken);

    public async Task<Result> IsOwner(Guid userId, Guid dashboardId, CancellationToken cancellationToken)
    {
        var deviceId = await respository.GetDeviceIdByDashboardId(dashboardId, cancellationToken);

        if (deviceId == default)
        {
            logger.LogWarning("[User: {UserId}] attempted to check ownership of non-existing dashboard: {DashboardId}.", userId, dashboardId);
            return Result.Failure(DashboardNotFound);
        }

        return await deviceAccessService.IsOwner(userId, deviceId, cancellationToken);
    }
}
