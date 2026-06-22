namespace Senswave.Integration.DataSource.BrokerConnection.Logging;

public record MessageIgnoredLog : BaseLog
{
    public string Topic { get; set; } = string.Empty;
}
