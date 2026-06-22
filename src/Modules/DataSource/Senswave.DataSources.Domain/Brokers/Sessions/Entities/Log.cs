using Senswave.DataSources.Domain.Brokers.Sessions.Enums;

namespace Senswave.DataSources.Domain.Brokers.Sessions.Entities;

public class Log : AuditableEntity
{
    public Session Session { get; set; }

    public SessionEventType Type { get; set; } = SessionEventType.Connected;

    [MaxLength(AllowedLengths.Descriptions.MaxLength)]
    public string Data { get; set; }
}
