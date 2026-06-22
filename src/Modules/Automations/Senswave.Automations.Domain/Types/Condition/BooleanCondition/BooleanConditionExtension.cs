using Senswave.Automations.Domain.Entities;
using System.Text.Json;

namespace Senswave.Automations.Domain.Types.Condition.BooleanCondition;

internal static class BooleanConditionExtension
{
    internal static BooleanCondition ToBooleanCondition(this AutomationCondition automationCondition)
    {
        return JsonSerializer
            .Deserialize<BooleanCondition>(automationCondition.ConditionConfiguration)!;
    }
}