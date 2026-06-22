using Senswave.Automations.Application.Features.AutomationState;

namespace Senswave.Automations.Api.Features.AutomationState;

internal static class AutomationStateExtension
{
    public static AutomationStateCommand ToCommand(this AutomationStateRequest request, Guid userId, Guid automationId) => new()
    {
        AutomationId = automationId,
        UserId = userId,
        IsEnabled = request.IsEnabled
    };
}