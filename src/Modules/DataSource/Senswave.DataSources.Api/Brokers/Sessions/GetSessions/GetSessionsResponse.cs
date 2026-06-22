namespace Senswave.DataSources.Api.Brokers.Sessions.GetSessions;

public record GetSessionsResponse
{
    public IEnumerable<SessionDto> Items { get; set; } = [];
}
