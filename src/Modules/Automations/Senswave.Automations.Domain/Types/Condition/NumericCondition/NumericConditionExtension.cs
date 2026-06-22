using Senswave.Automations.Domain.Entities;
using System.Text.Json;

namespace Senswave.Automations.Domain.Types.Condition.NumericCondition;

internal static class NumericConditionExtension
{
    internal static NumericCondition ToNumericCondition(this AutomationCondition automationCondition)
    {
        return JsonSerializer
            .Deserialize<NumericCondition>(automationCondition.ConditionConfiguration)!;
    }
}