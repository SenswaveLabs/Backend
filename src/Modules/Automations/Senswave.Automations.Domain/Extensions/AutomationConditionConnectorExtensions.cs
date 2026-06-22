using Senswave.Automations.Domain.Enums;

namespace Senswave.Automations.Domain.Extensions;

public static class AutomationConditionConnectorExtensions
{
    public static AutomationConditionConnector ToAutomationConditionConnector(this string conditionConnector) => conditionConnector switch
    {
        "And" => AutomationConditionConnector.And,
        "Or" => AutomationConditionConnector.Or,
        "" => AutomationConditionConnector.Empty,
        _ => AutomationConditionConnector.Invalid
    };
}
