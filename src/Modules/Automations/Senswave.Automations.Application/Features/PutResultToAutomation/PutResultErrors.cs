namespace Senswave.Automations.Application.Features.PutResultToAutomation;

public static class PutResultErrors
{
    public static Error AutomationNotFound
        => Error.NotFound("AutomationNotFound", "Automation with provided id does not found in the database");

    public static Error UserHasNoAccess => Error.Failure("UserHasNoAccess", "User has no access to the automation");

    public static Error OperationNotFound
        => Error.NotFound("OperationNotFound", "Operation with provided id does not found in the database");

    public static Error DatabaseError => Error.ServerError("DatabaseError", "Can not update automation with provided result");
}