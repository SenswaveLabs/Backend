using Senswave.Automations.Domain.Enums;

namespace Senswave.Automations.Domain.Entities;

public class Automation : AuditableEntity
{
    public Guid HomeReferenceId { get; set; } = Guid.Empty;
    public HomeReference HomesReference { get; set; } = new();

    [Required]
    [MinLength(AllowedLengths.Names.MinLength)]
    [MaxLength(AllowedLengths.Names.MaxLength)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(AllowedLengths.Icons.MaxLength)]
    public string Icon { get; set; } = string.Empty;

    public bool IsEnabled { get; set; } = true;

    public AutomationConditionConnector ConditionsConnector { get; set; }
    public IList<AutomationCondition> Conditions { get; set; } = [];
    public IList<AutomationResult> Results { get; set; } = [];
}
