using Senswave.DataSources.Domain.Brokers.Brokers.Repositories;
using Senswave.DataSources.Domain.Brokers.Clients.Enums;
using Senswave.DataSources.Domain.Brokers.Clients.Proxy;
using Senswave.Integration.DataSource.State;
using Senswave.Integration.Shared;

namespace Senswave.DataSources.Infrastructure.DataSources.Consumers;

internal sealed class DataSourcesStateConsumer(
    IBrokerQueryRepository repository,
    IClientProxy clientProxy,
    ILogger<DataSourcesStateConsumer> logger) : IConsumer<DataSourceStateRequest>
{
    public async Task Consume(ConsumeContext<DataSourceStateRequest> context)
    {
        var dataSource = await repository.GetBroker(context.Message.DataSourceReferenceId, context.CancellationToken);

        if (dataSource is null)
        {
            var badResponse = new DataSourceStateResponse()
            {
                StatusCode = InternalRequestStatus.Failure
            };

            logger.LogWarning("[DataSource: {dataSourceId}] Failed to find data source by id.", context.Message.DataSourceReferenceId);
            await context.RespondAsync(badResponse);
            return;
        }

        var client = await clientProxy.ClientState(context.Message.DataSourceReferenceId, context.CancellationToken);

        if (client.IsFailure)
        {
            var clientFailureRepsonse = new DataSourceStateResponse()
            {
                StatusCode = InternalRequestStatus.Success,
                Name = dataSource.Name,
                State = ClientState.NotStarted.ToString()
            };
            logger.LogInformation("[DataSource: {dataSourceId}] Failed to get client state for data source.", context.Message.DataSourceReferenceId);
            await context.RespondAsync(clientFailureRepsonse);

            return;
        }

        var response = new DataSourceStateResponse()
        {
            StatusCode = InternalRequestStatus.Success,
            Name = dataSource.Name,
            State = client.Data.ToString()
        };

        logger.LogInformation("[DataSource: {dataSourceId}] Deduced data source state: {state}", context.Message.DataSourceReferenceId, response.State);
        await context.RespondAsync(response);
    }
}
