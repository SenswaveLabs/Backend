namespace Senswave.Automations.Domain.Enums;

public enum AutomationConditionType
{
    Invalid = -1,
    Empty = 0,

    BooleanCondition = 1,
    NumberCondition = 2,
    TextCondition = 3
}
