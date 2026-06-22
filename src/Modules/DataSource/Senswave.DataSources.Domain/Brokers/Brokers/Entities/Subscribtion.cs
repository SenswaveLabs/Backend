namespace Senswave.DataSources.Domain.Brokers.Brokers.Entities;

public class Subscribtion : AuditableEntity
{
    public Guid BrokerId { get; set; }
    public Broker Broker { get; set; }

    [Required]
    [MinLength(1)]
    [MaxLength(1024)]
    public string Topic { get; set; } = string.Empty;
}
