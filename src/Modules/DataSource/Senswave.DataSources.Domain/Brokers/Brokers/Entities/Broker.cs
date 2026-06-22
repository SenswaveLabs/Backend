using Senswave.DataSources.Domain.Brokers.Brokers.ValueObjects;
using Senswave.DataSources.Domain.Brokers.Sessions.Entities;

namespace Senswave.DataSources.Domain.Brokers.Brokers.Entities;

public class Broker : AuditableEntity
{
    public Guid OwnerId { get; set; } = Guid.Empty;

    [Required]
    [MaxLength(AllowedLengths.Names.MaxLength)]
    [MinLength(AllowedLengths.Names.MinLength)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public BrokerInfo BrokerInfo { get; set; } = new();

    public IList<Subscribtion> Subscribtions { get; set; } = [];
    public IList<Session> Sessions { get; set; } = [];
}
