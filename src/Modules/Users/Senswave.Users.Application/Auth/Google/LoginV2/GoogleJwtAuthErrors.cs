namespace Senswave.Users.Application.Auth.Google.LoginV2;

internal class GoogleJwtAuthErrors
{
    public static Error FailedToRetriveGoogleToken = Error.Failure("FailedToRetriveGoogleToken", "Failed to authenticate with google using provided user code.");

    public static Error FailedToAuthenticate = Error.Failure("FailedToAuthenticate", "Failed to authenticate using google.");

    public static Error FailedToCreateUser = Error.Failure("FailedToCreateUser", "Failed to create user.");

}
