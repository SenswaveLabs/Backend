namespace Senswave.Abstractions.Entities;

public abstract class AuditableEntity : Entity
{
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
