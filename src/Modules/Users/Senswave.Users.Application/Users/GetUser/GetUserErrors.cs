namespace Senswave.Users.Application.Users.GetUser;

public class GetUserErrors
{
    public static Error UserNotFound => Error.NotFound("UserNotFound", "User not found.");
}
