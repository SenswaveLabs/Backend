using Senswave.DataSources.Domain.Brokers.Sessions.Entities;

namespace Senswave.DataSources.Application.Brokers.Sessions.GetSession;

public class GetSessionQuery : IQuery<Session>
{
    public Guid UserId { get; set; }
    public Guid BrokerId { get; set; }
    public Guid SessionId { get; set; }
}
