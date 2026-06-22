namespace Senswave.Automations.Application.Features.DisplayAutomations;

public static class DisplayAutomationsErrors
{
    public static Error AutomationsNotFound => Error.NotFound("AutomationsNotFound",
        "Automations for home with provided id not found or you do not have proper privilege to get them");
    public static Error HomeReferenceNotFound => Error.NotFound("HomeReferenceNotFound",
        "Home reference not found for provided home id");
}