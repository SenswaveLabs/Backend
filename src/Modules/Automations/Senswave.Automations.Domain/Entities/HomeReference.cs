namespace Senswave.Automations.Domain.Entities;

public class HomeReference : Entity
{
    public Guid OwnerId { get; set; }

    public Guid HomeId { get; set; }

    public List<Automation> Automations { get; set; } = [];
}