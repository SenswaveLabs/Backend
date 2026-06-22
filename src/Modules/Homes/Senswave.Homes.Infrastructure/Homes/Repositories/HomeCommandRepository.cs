using Microsoft.EntityFrameworkCore;
using Senswave.Homes.Domain.Homes.Entities;
using Senswave.Homes.Domain.Homes.Repositories;

namespace Senswave.Homes.Infrastructure.Homes.Repositories;

internal class HomeCommandRepository(
    ILogger<HomeCommandRepository> logger,
    HomesContext context) : IHomeCommandRepository
{
    public async Task<Result> CreateHome(Home home, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await context.Homes.AddAsync(home, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while creating home");
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.ServerError("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> RemoveHome(Guid homeId, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var home = await context.Homes
                .Where(h => h.Id == homeId)
                .FirstOrDefaultAsync(cancellationToken);

            if (home is null)
                return Result.Failure(Error.NotFound("HomeNotFound", "Home not found."));

            context.Homes.Remove(home);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception e)
        {
            logger.LogError(e, "[Home: {homeId}] Error while removing home", homeId);
            return Result.Failure(Error.ServerError("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> UpdateHome(Home home, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            context.Update(home);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception e)
        {
            logger.LogError(e, "[Home: {homeId}] Error while updating home", home.Id);
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.ServerError("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> AssignDataSourceToHome(Home home, Guid dataSourceId, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var reference = new DataSourceReference()
            {
                HomeId = home.Id,
                Home = home,
                DataSourceId = dataSourceId,
            };

            context.Homes.Attach(home);
            home.DataSourceReference = reference;

            await context.DataSourceReferences.AddAsync(reference, cancellationToken);
            context.Update(home);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception e)
        {
            logger.LogError(e, "[Home: {homeId}] Error while assigning data source to home.", home.Id);
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.ServerError("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> DeleteDataSourceFromHome(Home home, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            context.Remove(home.DataSourceReference!);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception e)
        {
            logger.LogError(e, "[Home: {homeId}] Error while assigning data source to home.", home.Id);
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.ServerError("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public Task<Home?> GetHome(Guid homeId, CancellationToken cancellationToken) => context.Homes
        .Where(h => h.Id == homeId)
        .Include(x => x.DataSourceReference)
        .FirstOrDefaultAsync(cancellationToken);

    public Task<bool> HomeExists(Guid userId, string name, CancellationToken cancellationToken) => context.Homes
        .Where(home => home.OwnerId == userId)
        .AnyAsync(home => home.Name == name, cancellationToken);

    public Task<int> CountHomeByDataSourceId(Guid dataSourceId, CancellationToken cancellationToken) => context.Homes
        .Where(home => home.DataSourceReference != null)
        .Where(home => home.DataSourceReference!.DataSourceId == dataSourceId)
        .CountAsync(cancellationToken);
}