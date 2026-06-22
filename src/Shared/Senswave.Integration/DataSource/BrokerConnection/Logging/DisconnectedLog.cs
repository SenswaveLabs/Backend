namespace Senswave.Integration.DataSource.BrokerConnection.Logging;

public record DisconnectedLog : BaseLog
{
    public string Reason { get; set; } = string.Empty;
}
