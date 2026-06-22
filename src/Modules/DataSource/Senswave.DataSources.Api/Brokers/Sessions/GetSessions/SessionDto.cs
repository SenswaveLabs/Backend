namespace Senswave.DataSources.Api.Brokers.Sessions.GetSessions;

public record SessionDto
{
    public Guid Id { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public bool Finished { get; set; }
}
