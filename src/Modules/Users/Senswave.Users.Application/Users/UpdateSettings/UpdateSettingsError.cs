namespace Senswave.Users.Application.Users.UpdateSettings;

internal class UpdateSettingsError
{
    public static Error UserNotFound => Error.NotFound("UserNotFound", "User not found.");
}
