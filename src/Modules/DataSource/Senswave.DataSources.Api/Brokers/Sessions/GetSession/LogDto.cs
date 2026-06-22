namespace Senswave.DataSources.Api.Brokers.Sessions.GetSession;

public class LogDto
{
    public Guid Id { get; set; }

    public string EventType { get; set; } = string.Empty;

    public string Data { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }
}
