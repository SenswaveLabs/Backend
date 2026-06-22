using Senswave.Automations.Domain.Entities;
using Senswave.Automations.Domain.Enums;
using Senswave.Automations.Domain.Types.Condition;
using Senswave.Automations.Domain.Types.Condition.BooleanCondition;
using Senswave.Automations.Domain.Types.Condition.NumericCondition;
using Senswave.Automations.Domain.Types.Condition.TextCondition;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Senswave.Automations.UnitTests.Conditions;

public class BaseTests
{

    protected BaseCondition ToBooleanAutomationImplementation(JsonObject config)
    {
        var condition = ToBooleanAutomationCondition(config);
        var impl = JsonSerializer
            .Deserialize<BooleanCondition>(condition.ConditionConfiguration)!;
        impl.OperationId = condition.OperationId;
        return impl;
    }

    protected AutomationCondition ToBooleanAutomationCondition(JsonObject config) => new()
    {
        ConditionConfiguration = config,
        ConditionType = AutomationConditionType.BooleanCondition,
        OperationId = Guid.NewGuid()
    };

    protected BaseCondition ToNumericAutomationImplementation(JsonObject config)
    {
        var condition = ToNumericAutomationCondition(config);
        var impl = JsonSerializer
            .Deserialize<NumericCondition>(condition.ConditionConfiguration)!;
        impl.OperationId = condition.OperationId;
        return impl;
    }

    protected AutomationCondition ToNumericAutomationCondition(JsonObject config) => new()
    {
        ConditionConfiguration = config,
        ConditionType = AutomationConditionType.NumberCondition,
        OperationId = Guid.NewGuid()
    };

    protected BaseCondition ToTextConditionImplementation(JsonObject config)
    {
        var condition = ToTextAutomationCondition(config);
        var impl = JsonSerializer
            .Deserialize<TextCondition>(condition.ConditionConfiguration)!;
        impl.OperationId = condition.OperationId;
        return impl;
    }

    protected AutomationCondition ToTextAutomationCondition(JsonObject config) => new()
    {
        ConditionConfiguration = config,
        ConditionType = AutomationConditionType.TextCondition,
        OperationId = Guid.NewGuid()
    };
}