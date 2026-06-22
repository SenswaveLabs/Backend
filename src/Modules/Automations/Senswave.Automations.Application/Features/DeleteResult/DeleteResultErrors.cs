namespace Senswave.Automations.Application.Features.DeleteResult;

public static class DeleteResultErrors
{
    public static Error ResultNotFound =>
        Error.NotFound("ResultNotFound", "Result with provided id was not found.");

    public static Error UserHasNoAccess => Error.Failure("UserHasNoAccess", "User has no access to the automation");

    public static Error CanNotDeleteResult => Error.Failure("CanNotDeleteResult",
        "Can not perform result deletion from database");

    public static Error CanNotDeleteResultIfThereIsOnlyResultInAutomation => Error.Failure(
        "CanNotDeleteResultIfThereIsOnlyResultInAutomation",
        "Can not perform Result deletion from Automation if there is only one Result");
}