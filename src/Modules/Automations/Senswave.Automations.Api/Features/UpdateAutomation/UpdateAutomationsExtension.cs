using Senswave.Automations.Api.Features.CreateAutomation;
using Senswave.Automations.Application.Features.UpdateAutomation;
using Senswave.Automations.Domain.Entities;
using Senswave.Automations.Domain.Extensions;

namespace Senswave.Automations.Api.Features.UpdateAutomation;

internal static class UpdateAutomationsExtension
{
    public static UpdateAutomationCommand ToCommand(this UpdateAutomationRequest request, Guid userId, Guid automationId) => new()
    {
        UserId = userId,
        AutomationId = automationId,
        Name = request.Name,
        Icon = request.Icon,
        ConditionConnector = request.ConditionConnector.ToAutomationConditionConnector(),
        Conditions = request.Conditions.Select(x => x.ToCondition()).ToList(),
        Results = request.Results.Select(x => x.ToResult()).ToList()
    };

    public static AutomationResult ToResult(this ResultDto resultItemDto) => new()
    {
        OperationId = resultItemDto.OperationId,
        ValueToSend = resultItemDto.ValueToSend
    };

    public static AutomationCondition ToCondition(this ConditionDto conditionItemDto) => new()
    {
        OperationId = conditionItemDto.OperationId,
        ConditionConfiguration = conditionItemDto.ConditionConfiguration,
        ConditionType = conditionItemDto.ConditionType.ToAutomationConditionType()
    };
}