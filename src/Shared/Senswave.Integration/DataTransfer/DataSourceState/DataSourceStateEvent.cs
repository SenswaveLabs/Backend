namespace Senswave.Integration.DataTransfer.DataSourceState;

public class DataSourceStateEvent
{
    public Guid DataSourceId { get; set; }
    public string State { get; set; } = string.Empty;
}
