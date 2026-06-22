namespace Senswave.Homes.Application.Sharings.Features.CreateSharing;

public static class CreateSharingErrors
{
    public static Error UserNotFound =>
        Error.NotFound("UserNotFound", "User with provided id not found in the database");

    public static Error HomeNotFound =>
        Error.NotFound("HomeNotFound", "Home with provided id not found in the database");

    public static Error UserIsOwner => Error.Conflict("UserIsOwner", "User is the owner of the home");

    public static Error InvitationAlreadyExists => Error.Conflict("InvitationAlreadyExists", "An invitation for this user already exists.");
}