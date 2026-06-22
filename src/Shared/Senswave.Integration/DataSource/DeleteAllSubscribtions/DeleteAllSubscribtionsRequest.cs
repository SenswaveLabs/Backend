namespace Senswave.Integration.DataSource.DeleteAllSubscribtions;

public record DeleteAllSubscribtionsRequest
{
    public Guid DataSourceId { get; set; }
}
