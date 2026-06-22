namespace Senswave.Automations.Application.Features.UpdateAutomation;

public static class UpdateAutomationErrors
{
    public static Error AutomationUpdateFailed =>
        Error.ServerError("AutomationUpdateFailed", "Failed to update automation in the database");

    public static Error AutomationNotFound =>
        Error.NotFound("AutomationNotFound", "Automation with provided id not found in the database");

    public static Error AccessDenied =>
        Error.Failure("AccessDenied", "Access to automation is denied");

    public static Error ConditionConfigurationValidationError =>
        Error.Validation("ConditionConfigurationValidationError", "Conditions configuration has got wrong format");
}