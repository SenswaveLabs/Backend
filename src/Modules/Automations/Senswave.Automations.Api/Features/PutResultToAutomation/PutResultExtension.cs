using Senswave.Automations.Application.Features.PutResultToAutomation;
using Senswave.Automations.Domain.Entities;
using System.Text.Json.Nodes;

namespace Senswave.Automations.Api.Features.PutResultToAutomation;

internal static class PutResultExtension
{
    public static PutResultCommand
        ToCommand(this Guid automationId, Guid userId, Guid operationId, String valueToSend) => new()
        {
            AutomationId = automationId,
            UserId = userId,
            Result = new AutomationResult { OperationId = operationId, ValueToSend = JsonValue.Create(valueToSend) }
        };
}