namespace Senswave.Users.Infrastructure.Errors;

public class ExternalUserServiceErrors
{
    public readonly static Error FailedToFindUser = Error.Failure("FailedToFindUser", "Failed to find user.");

    public readonly static Error ExternalUserLinkingFailed = Error.ServerError("ExternalUserLinkingFailed", "An error occurred while linking external user.");

    public readonly static Error FailedToCreateUser = Error.Failure("FailedToCreateUser", "Failed to create user.");

    public readonly static Error FailedToAttachUserInfo = Error.Failure("FailedToAttachUserInfo", "Failed to attach registration info to user.");

    public readonly static Error ExternalUserRegistrationFailed = Error.ServerError("ExternalUserRegistrationFailed", "An error occurred while registering external user.");
}
