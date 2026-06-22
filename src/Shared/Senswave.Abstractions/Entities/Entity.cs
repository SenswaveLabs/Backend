using System.ComponentModel.DataAnnotations;

namespace Senswave.Abstractions.Entities;

public abstract class Entity
{
    [Key]
    public Guid Id { get; set; } = Guid.CreateVersion7();
}
