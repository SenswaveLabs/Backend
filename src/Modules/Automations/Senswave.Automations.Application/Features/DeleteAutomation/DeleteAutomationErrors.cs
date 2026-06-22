namespace Senswave.Automations.Application.Features.DeleteAutomation;

public static class DeleteAutomationErrors
{
    public static Error AutomationNotFound =>
        Error.NotFound("AutomationNotFound", "Automation with provided id not found in the database");

    public static Error UserHasNoAccess => Error.Failure("UserHasNoAccess", "User has no access to the automation");

    public static Error CanNotDeleteAutomation => Error.Failure("CanNotDeleteAutomation",
        "Can not perform automation deletion due to problem with database");

    public static Error CanNotDeleteHomeReference => Error.Failure("CanNotDeleteHomeReference",
        "Can not delete home reference due to problem with database");
}