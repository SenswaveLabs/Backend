using Senswave.Automations.Domain.Entities;
using Senswave.Automations.Domain.Enums;

namespace Senswave.Automations.Application.Features.CreateAutomation;

public class CreateAutomationCommand : ICommand<Automation>
{
    public Guid UserId { get; set; } = Guid.Empty;
    public Guid HomeId { get; set; } = Guid.Empty;

    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;

    public bool IsEnabled { get; set; } = true;

    public AutomationConditionConnector ConditionConnector { get; set; } = AutomationConditionConnector.Invalid;

    public IList<AutomationCondition> Conditions = [];

    public IList<AutomationResult> Results = [];
}