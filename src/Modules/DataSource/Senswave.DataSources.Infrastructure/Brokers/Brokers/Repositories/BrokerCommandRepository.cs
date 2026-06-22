using Microsoft.EntityFrameworkCore;
using Senswave.DataSources.Domain.Brokers.Brokers.Entities;
using Senswave.DataSources.Domain.Brokers.Brokers.Repositories;

namespace Senswave.DataSources.Infrastructure.Brokers.Brokers.Repositories;

internal class BrokerCommandRepository(
    DataSourcesContext context,
    ILogger<BrokerCommandRepository> logger) : IBrokerCommandRepository
{
    public async Task<Result> CreateBroker(Broker broker, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await context.Brokers.AddAsync(broker, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Broker: {brokerId}] Error while deleting broker.", broker.Id);
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.ServerError("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> UpdateBroker(Broker broker, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            broker.UpdatedAtUtc = DateTime.UtcNow;
            context.Update(broker);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Broker: {brokerId}] Error while deleting broker.", broker.Id);
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.ServerError("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> DeleteBroker(Broker broker, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            context.Brokers.Remove(broker);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Broker: {brokerId}] Error while deleting broker.", broker.Id);
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.ServerError("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public Task<Broker?> GetBroker(Guid brokerId, Guid userId, CancellationToken cancellationToken)
        => context.Brokers
        .Where(b => b.Id == brokerId)
        .Where(b => b.OwnerId == userId)
        .AsNoTracking()
        .FirstOrDefaultAsync(cancellationToken);
}