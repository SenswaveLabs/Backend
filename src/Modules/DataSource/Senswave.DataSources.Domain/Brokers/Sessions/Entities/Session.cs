using Senswave.DataSources.Domain.Brokers.Brokers.Entities;

namespace Senswave.DataSources.Domain.Brokers.Sessions.Entities;

public class Session : AuditableEntity
{
    public Guid BrokerId { get; set; }
    public Broker Broker { get; set; }

    public IList<Log> Logs { get; set; } = [];

    public bool Finished { get; set; }
}
