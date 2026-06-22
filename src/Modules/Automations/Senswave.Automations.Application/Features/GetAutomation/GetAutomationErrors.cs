namespace Senswave.Automations.Application.Features.GetAutomation;

public static class GetAutomationErrors
{
    public static Error AutomationNotFound =>
        Error.NotFound("AutomationNotFound", "Automation with provided id not found in the database");

    public static Error AccessDenied => Error.Failure("AccessDenied", "You can not display this automation");
}