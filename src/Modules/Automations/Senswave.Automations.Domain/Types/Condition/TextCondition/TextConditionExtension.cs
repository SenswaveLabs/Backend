using Senswave.Automations.Domain.Entities;
using System.Text.Json;

namespace Senswave.Automations.Domain.Types.Condition.TextCondition;

internal static class TextConditionExtension
{
    internal static TextCondition ToTextCondition(this AutomationCondition automationCondition)
    {
        return JsonSerializer
            .Deserialize<TextCondition>(automationCondition.ConditionConfiguration)!;
    }
}