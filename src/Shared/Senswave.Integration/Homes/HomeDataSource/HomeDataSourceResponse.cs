using Senswave.Integration.Shared;

namespace Senswave.Integration.Homes.HomeDataSource;

/*
 * Message send to Homes modules to get DataSourceId
 */
public record HomeDataSourceResponse : BaseInternalResponse
{
    public Guid DataSourceId { get; set; } = Guid.Empty;
}