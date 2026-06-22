using Senswave.Automations.Application.Features.DeleteCondition;

namespace Senswave.Automations.Api.Features.DeleteCondition;

internal static class DeleteConditionExtensions
{
    public static DeleteConditionCommand ToDeleteConditionCommand(this Guid conditionId, Guid userId) => new()
    {
        ConditionId = conditionId,
        UserId = userId
    };
}