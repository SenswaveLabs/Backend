using Senswave.DataSources.Domain.Brokers.Brokers.Entities;

namespace Senswave.DataSources.Application.Brokers.Brokers.Features.GetSubscribtions;

public class GetSubscribtionsQuery : IPagedQuery<IEnumerable<Subscribtion>>
{
    public Guid UserId { get; set; } = Guid.Empty;
    public Guid BrokerId { get; set; } = Guid.Empty;
    public int Page { get; set; }
    public int Size { get; set; }
}
