using Senswave.Automations.Domain.Entities;
using Senswave.Automations.Domain.Enums;
using Senswave.Automations.Domain.Factory;
using Senswave.Automations.Domain.Services;
using Senswave.Integration.Devices.LastOperationValue;
using System.Text.Json.Nodes;

namespace Senswave.Automations.Application.Services;

public class AutomationInterpreter(
    IRequestClient<LastOperationValueRequest> operationValueProvider,
    ConditionFactory conditionFactory)
    : IAutomationInterpreter
{
    public async Task<bool> Interpret(Automation automation, CancellationToken cancellationToken)
    {
        foreach (var condition in automation.Conditions)
        {
            var lastValueMessage =
                await operationValueProvider.GetResponse<LastOperationValueResponse>(
                    LastValueRequest(condition.OperationId));

            if (lastValueMessage.Message.IsFailure)
                return false;

            var conditionEvaluation = await InterpretCondition(
                condition,
                lastValueMessage.Message.LastValue.AsValue(),
                cancellationToken);

            // ConditionConnector is Or => Only on true operation is enough to make whole condition true
            if (conditionEvaluation && automation.ConditionsConnector == AutomationConditionConnector.Or)
                return true;

            // ConditionConnector is And => Only one false operation is enough to make whole condition false
            if (!conditionEvaluation && automation.ConditionsConnector == AutomationConditionConnector.And)
                return false;
        }

        // ConditionConnector is Or and all conditionEvaluation are false
        if (automation.ConditionsConnector == AutomationConditionConnector.Or)
            return false;

        // ConditionConnector is And and all conditionEvaluation are true
        return true;
    }

    private async Task<bool> InterpretCondition(AutomationCondition automationCondition, JsonValue operationPayload, CancellationToken cancellationToken)
    {
        var condition = await conditionFactory.Create(automationCondition, cancellationToken);

        if (condition.IsFailure)
            return false;

        return condition.Data.CheckCondition(operationPayload);
    }

    private static LastOperationValueRequest LastValueRequest(Guid operationId) => new()
    {
        OperationId = operationId
    };
}