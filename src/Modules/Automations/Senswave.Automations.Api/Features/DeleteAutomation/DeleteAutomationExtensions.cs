using Senswave.Automations.Application.Features.DeleteAutomation;

namespace Senswave.Automations.Api.Features.DeleteAutomation;

internal static class DeleteAutomationExtensions
{
    public static DeleteAutomationCommand ToDeleteCommand(this Guid automationId, Guid userId) => new()
    {
        AutomationId = automationId,
        UserId = userId
    };
}
