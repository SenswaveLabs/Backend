using Senswave.DataSources.Domain.Brokers.Sessions.Entities;

namespace Senswave.DataSources.Application.Brokers.Sessions.GetSessions;

public class GetSessionsQuery : IPagedQuery<IEnumerable<Session>>
{
    public int Page { get; set; }
    public int Size { get; set; }
    public Guid BrokerId { get; set; }
    public Guid UserId { get; set; }
}
