namespace Senswave.Homes.Application.Sharings.Features.AcceptSharing;

internal static class AcceptSharingErrors
{
    internal static Error TooManyUsersInHome =>
        Error.Conflict("TooManyUsersInHome", "Too many users in the home.");

    internal static Error InvitationNotFound =>
        Error.NotFound("InvitationNotFound", "Invitation not found in the database.");

    internal static Error NoActiveInvitations =>
        Error.NotFound("NoActiveInvitations", "You don't have any invitation.");

    internal static Error InvitationExpired =>
        Error.Validation("InvitationExpired", "Invitation has expired, please request a new one.");
}