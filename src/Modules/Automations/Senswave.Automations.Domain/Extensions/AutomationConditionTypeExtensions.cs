using Senswave.Automations.Domain.Enums;

namespace Senswave.Automations.Domain.Extensions;

public static class AutomationConditionTypeExtensions
{
    public static AutomationConditionType ToAutomationConditionType(this string conditionType) => conditionType switch
    {
        "BooleanCondition" => AutomationConditionType.BooleanCondition,
        "NumberCondition" => AutomationConditionType.NumberCondition,
        "TextCondition" => AutomationConditionType.TextCondition,
        "" => AutomationConditionType.Empty,
        _ => AutomationConditionType.Invalid
    };
}
