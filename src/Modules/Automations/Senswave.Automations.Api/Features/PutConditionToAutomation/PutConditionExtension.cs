using Senswave.Automations.Application.Features.PutConditionToAutomation;
using Senswave.Automations.Domain.Entities;
using Senswave.Automations.Domain.Extensions;
using System.Text.Json.Nodes;

namespace Senswave.Automations.Api.Features.PutConditionToAutomation;

internal static class PutConditionExtension
{
    public static PutConditionCommand ToCommand(
        this Guid automationId,
        Guid userId,
        Guid operationId,
        string conditionType,
        JsonObject conditionConfiguration) =>
        new PutConditionCommand
        {
            AutomationId = automationId,
            UserId = userId,
            AutomationCondition = new AutomationCondition
            {
                OperationId = operationId,
                ConditionType = conditionType.ToAutomationConditionType(),
                ConditionConfiguration = conditionConfiguration
            }
        };
}