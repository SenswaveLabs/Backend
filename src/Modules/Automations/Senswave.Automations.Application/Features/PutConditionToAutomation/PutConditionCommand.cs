using Senswave.Automations.Domain.Entities;

namespace Senswave.Automations.Application.Features.PutConditionToAutomation;

public class PutConditionCommand : ICommand
{
    public AutomationCondition? AutomationCondition { get; set; } = null;
    public Guid AutomationId { get; set; } = Guid.Empty;

    public Guid UserId { get; set; } = Guid.Empty;
}