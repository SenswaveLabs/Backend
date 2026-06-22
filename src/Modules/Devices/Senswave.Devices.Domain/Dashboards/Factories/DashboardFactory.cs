using Senswave.Devices.Domain.Dashboards.Entities;
using Senswave.Devices.Domain.Dashboards.Enums;
using Senswave.Devices.Domain.Dashboards.Repositories;
using Senswave.Devices.Domain.Dashboards.Types;
using Senswave.Devices.Domain.Dashboards.Types.Gird;
using Senswave.Devices.Domain.Widgets.Factory;
using System.Text.Json;

namespace Senswave.Devices.Domain.Dashboards.Factories;


public class DashboardFactory(
    IDashboardsQueryRespository queryRespository,
    WidgetFactory widgetFactory,
    ILogger<DashboardFactory> logger)
{
    #region Errors

    private static Error FailedToInitializeDashboard => Error.Failure("FailedToInitializeDashboard", "Failed to initialize dashboard.");

    private static Error FailedToCreateDashboard => Error.Failure("FailedToCreateDashboard", "Failed to create dashboard.");

    private static Error DashboardValidationFailed => Error.Failure("DashboardValidationFailed", "Failed to validate dashboard");

    #endregion

    public async Task<Result<IDashboard>> Initialize(Guid deviceId, string name, string icon, DashboardType type, JsonObject configuration)
    {
        var dashboard = new Dashboard
        {
            DeviceId = deviceId,

            Name = name,
            Icon = icon,
            Type = type,
            Configuration = configuration,
        };

        var createDashboardResult = Create(dashboard, true);

        if (createDashboardResult.IsFailure)
        {
            logger.LogError("[Dashboard Type: {dashboardType}] Failed to create new dashboard.",
                dashboard.Type);

            return Result<IDashboard>.Failure(FailedToInitializeDashboard);
        }

        var validationResult = await createDashboardResult.Data.Validate();

        if (validationResult.IsFailure)
        {
            logger.LogError("[Dashboard Type: {dashboardType}] Failed to validate new dashboard.",
                dashboard.Type);

            return Result<IDashboard>.Failure(DashboardValidationFailed);
        }

        return Result<IDashboard>.Success(createDashboardResult.Data);
    }

    public async Task<Result<IDashboard>> Create(Guid dashboardId, CancellationToken cancellationToken)
    {
        try
        {
            var dashboard = await queryRespository
                .GetDashboard(dashboardId, cancellationToken);

            if (dashboard is null)
                return Result<IDashboard>.Failure(Error.NotFound("DashboardNotFound", "Dashboard not found."));

            return Create(dashboard);
        }
        catch (Exception e)
        {
            logger.LogError(e, "[Dashboard: {dashboardId}] Failed to create dashboard.",
                dashboardId);

            return Result<IDashboard>.Failure(FailedToCreateDashboard);
        }
    }

    private Result<IDashboard> Create(Dashboard dashboard, bool initialize = false)
    {
        try
        {
            if (dashboard.Type == DashboardType.Grid)
            {
                var configuration = JsonSerializer.Deserialize<GridDashboardConfiguration>(dashboard.Configuration)!;

                var grid = new GridDashboard(dashboard, configuration, widgetFactory, initialize);

                return Result<IDashboard>.Success(grid);
            }

            logger.LogError("[Dashboard Type: {dashboardType}] Unknown dashboard type when creating.",
                dashboard.Type);

            return Result<IDashboard>.Failure(Error.Failure("UnknownDashboardType", "Unknown dashboard type."));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Dashboard: {dashboard}] [Dashboard Type: {dashboardType}] Failed to initialize dashboard.",
                dashboard.Id,
                dashboard.Type);

            return Result<IDashboard>.Failure(FailedToCreateDashboard);
        }
    }
}
