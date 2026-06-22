using Senswave.Automations.Domain.Entities;
using Senswave.Automations.Domain.Enums;

namespace Senswave.Automations.Application.Features.UpdateAutomation;

public class UpdateAutomationCommand : ICommand
{
    public Guid AutomationId { get; set; } = Guid.Empty;
    public Guid UserId { get; set; } = Guid.Empty;


    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;


    public AutomationConditionConnector ConditionConnector { get; set; } = AutomationConditionConnector.Empty;

    public IList<AutomationCondition> Conditions = [];

    public IList<AutomationResult> Results = [];
}