using Microsoft.EntityFrameworkCore;
using Senswave.Devices.Domain.Dashboards.Entities;
using Senswave.Devices.Domain.Dashboards.Repositories;

namespace Senswave.Devices.Infrastructure.Dashboards.Repositories;

internal sealed class DashboardCommandRepository(
    DevicesContext context,
    ILogger<DashboardCommandRepository> logger) : IDashboardCommandRepository
{
    public async Task<Result> CreateDashboard(Dashboard dashboard, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await context.Dashboards.AddAsync(dashboard, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(ex, "Failed to create dashboard.");
            return Result.Failure(Error.ServerError("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> UpdateDashboard(Dashboard dashboard, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            context.Dashboards.Attach(dashboard);
            context.Dashboards.Update(dashboard);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(ex, "Failed to create dashboard.");

            return Result.Failure(Error.ServerError("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> DeleteDashboard(Guid dashboardId, CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var dashboard = await context.Dashboards
                .FirstOrDefaultAsync(x => x.Id == dashboardId, cancellationToken);

            if (dashboard == null)
                return Result.Failure(Error.NotFound("DashboardNotFound", "Dashboard not found."));

            context.Dashboards.Remove(dashboard);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(ex, "[Dashborad: {dashboardId}] Failed to delete dashboard with widgets.", dashboardId);
            return Result.Failure(Error.ServerError("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public Task<Dashboard?> GetDashboard(Guid dashboardId, CancellationToken cancellationToken) => context.Dashboards
        .FirstOrDefaultAsync(x => x.Id == dashboardId, cancellationToken);
}