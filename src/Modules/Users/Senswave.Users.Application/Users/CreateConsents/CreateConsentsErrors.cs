namespace Senswave.Users.Application.Users.CreateConsents;

public class CreateConsentsErrors
{
    public static Error UserNotFound = Error.ServerError("UserNotFound", "Couldn't accept condtions please try again.");

    public static Error FailedToAcceptConditions = Error.ServerError("FailedToAcceptConditions", "Couldn't accept condtions please try again.");
}
