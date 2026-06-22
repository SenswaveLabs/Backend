using Senswave.Abstractions.Resulting;
using Senswave.Automations.Application.Features.CreateAutomation;
using Senswave.Automations.Domain.Entities;
using Senswave.Automations.Domain.Extensions;

namespace Senswave.Automations.Api.Features.CreateAutomation;

internal static class CreateAutomationsExtension
{
    public static CreateAutomationCommand ToCommand(this CreateAutomationRequest request, Guid userId) => new()
    {
        UserId = userId,
        HomeId = request.HomeId,

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

    public static AutomationCreatedResponse ToAutomationCreatedResponse(this Result<Automation> automation) => new()
    {
        Id = automation.Data.Id
    };
}