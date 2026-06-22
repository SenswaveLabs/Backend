namespace Senswave.Automations.Application.Features.CreateAutomation;

public static class CreateAutomationErrors
{
    public static Error AutomationsLimitReached =>
        Error.Conflict("AutomationsLimitReached", "Automations limit reached");

    public static Error FailedToCreate =>
        Error.ServerError("FailedToCreate", "Failed to create automation");

    public static Error AccessDenied =>
        Error.NotFound("AccessDenied", "Access to home/devices not granted");

    public static Error NameAlreadyExists =>
        Error.Conflict("NameAlreadyExists", "Automation with given name already exists in the home");

    public static Error ConditionConfigurationValidationError =>
        Error.Validation("ConditionConfigurationValidationError", "Conditions configuration has got wrong format");

    public static Error FailedToGetHomeOwner =>
        Error.NotFound("FailedToGetHomeOwner", "Failed to get home owner information");
}