using Microsoft.EntityFrameworkCore;
using Senswave.DataSources.Domain.Brokers.Clients.Proxy;
using Senswave.Integration.DataSource.Remove;
using Senswave.Integration.Shared;

namespace Senswave.DataSources.Infrastructure.Consumers;

public class DataSourcesRemoveConsumer(
    IClientProxy clientProxy,
    DataSourcesContext dataSourcesContext,
    ILogger<DataSourcesRemoveConsumer> logger) : IConsumer<DataSourcesRemoveRequest>
{
    public async Task Consume(ConsumeContext<DataSourcesRemoveRequest> context)
    {
        try
        {
            var brokers = await dataSourcesContext.Brokers
                .Where(hr => hr.OwnerId == context.Message.UserId)
                .ToListAsync(context.CancellationToken);

            bool flowControl = await StopBrokers(context, brokers);

            if (!flowControl)
                return;

            var subscribtion = await dataSourcesContext.Subscribtions
                .Where(hs => hs.Broker.OwnerId == context.Message.UserId)
                .ToListAsync(context.CancellationToken);

            var sessions = await dataSourcesContext.Sessions
                .Where(hs => hs.Broker.OwnerId == context.Message.UserId)
                .ToListAsync(context.CancellationToken);

            var logs = await dataSourcesContext.Logs
                .Where(hs => hs.Session.Broker.OwnerId == context.Message.UserId)
                .ToListAsync(context.CancellationToken);

            dataSourcesContext.Brokers.RemoveRange(brokers);
            dataSourcesContext.Subscribtions.RemoveRange(subscribtion);
            dataSourcesContext.Sessions.RemoveRange(sessions);
            dataSourcesContext.Logs.RemoveRange(logs);

            await dataSourcesContext.SaveChangesAsync(context.CancellationToken);

            _ = StopBrokers(context, brokers);

            await context.RespondAsync<DataSourcesRemoveResponse>(BaseInternalResponse.Success());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[User: {userId}] Error occurred while removing data sources.", context.Message.UserId);
            var response = BaseInternalResponse.Failure(Error.ServerError("HomesRemovalServerError", "Failed to remove homes or linked entites. Please try again or remove datasources manually.")) as DataSourcesRemoveResponse;
            await context.RespondAsync(response!);
        }
    }

    private async Task<bool> StopBrokers(ConsumeContext<DataSourcesRemoveRequest> context, List<Domain.Brokers.Brokers.Entities.Broker> brokers)
    {
        foreach (var broker in brokers)
        {
            logger.LogDebug("[UserId: {userId}] [Broker: {brokerId}] Removing broker for user.", context.Message.UserId, broker.Id);

            var result = await clientProxy.Stop(broker.Id, context.CancellationToken);

            if (result.IsFailure && result.Errors.Length >= 1 && result.Errors.First().Code != "ClientDoesNotExist")
            {
                logger.LogError("[UserId: {userId}] [Broker: {brokerId}] Failed to stop broker before removal.", context.Message.UserId, broker.Id);
                return false;
            }
        }

        return true;
    }
}
