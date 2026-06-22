namespace Senswave.DataSources.Api.Brokers.Sessions.GetSession;

public record GetSessionResponse
{
    public Guid Id { get; set; } = Guid.Empty;

    public IEnumerable<LogDto> Logs { get; set; } = [];

    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }

}
