namespace Senswave.Integration.DataSource.BrokerConnection.Logging;

public record ReconnectingLog : BaseLog
{
    public string Reason { get; set; } = string.Empty;
}
