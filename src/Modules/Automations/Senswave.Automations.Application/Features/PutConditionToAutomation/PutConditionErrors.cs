namespace Senswave.Automations.Application.Features.PutConditionToAutomation;

public static class PutConditionErrors
{
    public static Error AutomationNotFound
        => Error.NotFound("AutomationNotFound", "Automation with provided id does not found in the database");
    public static Error UserHasNoAccess => Error.Failure("UserHasNoAccess", "User has no access to the automation");
    public static Error OperationNotFound
        => Error.NotFound("OperationNotFound", "Operation with provided id does not found in the database");
    public static Error ConditionConfigurationValidationError =>
        Error.Validation("ConditionConfigurationValidationError", "Conditions configuration has got wrong format");
    public static Error DatabaseError => Error.ServerError("DatabaseError", "Can not update automation with provided condition");

}