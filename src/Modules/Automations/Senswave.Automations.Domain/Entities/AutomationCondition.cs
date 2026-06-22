using Senswave.Automations.Domain.Enums;

namespace Senswave.Automations.Domain.Entities;

public class AutomationCondition : AuditableEntity
{
    public Automation Automation { get; set; }
    public Guid OperationId { get; set; }
    public AutomationConditionType ConditionType { get; set; }

    /*
     Examples of ConditionConfiguration for different types of conditions
     * Boolean -> {"isOn": "true"}
     * Numeric -> {"minValue": 21, "maxValue" : 37}
     * TextCondition -> {"requiredValue": żółć}
     */
    public JsonObject ConditionConfiguration { get; set; }
}
