using Microsoft.EntityFrameworkCore;
using Senswave.DataSources.Domain.Brokers.Brokers.Entities;
using Senswave.DataSources.Domain.Brokers.Brokers.Repositories;

namespace Senswave.DataSources.Infrastructure.Brokers.Brokers.Repositories;

internal class SubscribtionCommandRepository(
    DataSourcesContext context,
    ILogger<SubscribtionCommandRepository> logger) : ISubscribtionCommandRepository
{
    public async Task<Result> CreateSubscribtion(Guid brokerId, string topic, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var subscribtion = new Subscribtion
            {
                BrokerId = brokerId,
                Topic = topic
            };

            await context.Subscribtions.AddAsync(subscribtion, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Broker: {brokerId}][Topic: {topic}] Failed to create subscribtion for broker.", brokerId, topic);
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.ServerError("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public Task<Subscribtion?> GetSubscriptionWithBroker(Guid subscriptionId, CancellationToken cancellationToken) => context.Subscribtions
        .Include(s => s.Broker)
        .FirstOrDefaultAsync(x => x.Id == subscriptionId, cancellationToken);

    public Task<Subscribtion?> GetSubscriptionByTopic(Guid brokerId, string topic, CancellationToken cancellationToken) => context.Subscribtions
        .Include(s => s.Broker)
        .Where(s => s.Broker.Id == brokerId)
        .Where(s => s.Topic == topic)
        .FirstOrDefaultAsync(cancellationToken);

    public async Task<Result> DeleteSubscribtion(Guid subscriptionId, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var subscribtion = await context.Subscribtions
                .FirstOrDefaultAsync(x => x.Id == subscriptionId, cancellationToken);

            if (subscribtion is null)
                return Result.Failure(Error.NotFound("SubscribtionNotFound", "The subscription was not found."));

            context.Subscribtions.Remove(subscribtion);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Subscription: {subscriptionId}] Failed to delete subscribtion.", subscriptionId);
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.ServerError("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> DeleteAllSubscribtions(Guid dataSourceId, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var subscribtions = await context.Subscribtions.Where(x => x.BrokerId == dataSourceId).ToListAsync(cancellationToken);
            context.Subscribtions.RemoveRange(subscribtions);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Data Source: {brokerId}] Failed to remove all subscribtion for data source.", dataSourceId);
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.ServerError("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }
}
