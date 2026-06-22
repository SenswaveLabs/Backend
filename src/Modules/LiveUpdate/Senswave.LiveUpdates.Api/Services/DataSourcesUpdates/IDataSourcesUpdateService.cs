namespace Senswave.LiveUpdates.Api.Services.DataSourcesUpdates;

public interface IDataSourcesUpdateService
{
    Task UpdateDataSourceState(Guid dataSourceId, string state, CancellationToken cancellationToken);
}
