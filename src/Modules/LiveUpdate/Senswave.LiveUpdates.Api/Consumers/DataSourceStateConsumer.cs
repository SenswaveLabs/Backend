using Senswave.Integration.DataTransfer.DataSourceState;
using Senswave.LiveUpdates.Api.Services.DataSourcesUpdates;

namespace Senswave.LiveUpdates.Api.Consumers;

internal sealed class DataSourceStateConsumer(
    IDataSourcesUpdateService service,
    ILogger<DataSourceStateConsumer> logger) : IConsumer<DataSourceStateEvent>
{
    public async Task Consume(ConsumeContext<DataSourceStateEvent> context)
    {
        try
        {
            var dataSourceId = context.Message.DataSourceId;
            var state = context.Message.State;

            await service.UpdateDataSourceState(dataSourceId, state, context.CancellationToken);

            logger.LogInformation("[DataSource: {deviceID}] Device update for data source state: {state}",
                dataSourceId, state);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[DataSource: {deviceID}] Failed to send device update", context.Message.DataSourceId);
        }
    }
}
