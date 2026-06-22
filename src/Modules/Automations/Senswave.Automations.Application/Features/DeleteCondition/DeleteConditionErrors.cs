namespace Senswave.Automations.Application.Features.DeleteCondition;

public static class DeleteConditionErrors
{
    public static Error AutomationConditionNotFound => Error.NotFound("AutomationConditionNotFound",
        "Automation Condition with provided id not found in the database");
    public static Error UserHasNoAccess => Error.Failure("UserHasNoAccess",
        "User has no access to the automation");

    public static Error DeleteLastCondition => Error.Failure("DeleteLastCondition",
        "User can not delete last condition from automation");

    public static Error CanNotDeleteCondition => Error.Failure("CanNotDeleteCondition",
        "Can not perform automation condition deletion from database");
}