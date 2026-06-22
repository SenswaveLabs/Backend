using Senswave.DataSources.Domain.Brokers.Brokers.Entities;

namespace Senswave.DataSources.Application.Brokers.Brokers.Features.GetBrokers;

public class GetBrokersQuery : IPagedQuery<IEnumerable<Broker>>
{
    public Guid UserId { get; set; } = Guid.Empty;
    public int Page { get; set; }
    public int Size { get; set; }
}
