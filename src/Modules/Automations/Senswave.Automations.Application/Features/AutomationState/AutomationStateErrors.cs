namespace Senswave.Automations.Application.Features.AutomationState;

public static class AutomationStateErrors
{
    public static Error UpdateFailed =>
        Error.Failure("UpdateFailed", "Failed to update automation");

    public static Error AutomationNotFound =>
        Error.NotFound("AutomationNotFound", "Automation with provided id not found in the database");

    public static Error AccessDenied =>
        Error.Failure("AccessDenied", "User does not have access to this automation");
}