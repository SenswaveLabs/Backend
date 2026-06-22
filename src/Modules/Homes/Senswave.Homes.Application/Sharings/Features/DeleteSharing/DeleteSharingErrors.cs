namespace Senswave.Homes.Application.Sharings.Features.DeleteSharing;

public static class DeleteSharingErrors
{
    public static Error HomeSharingNotFound
        => Error.NotFound("HomeSharingNotFound", "HomeSharing with provided Id does not found in the database");

    public static Error FailedToRemoveSharing
        => Error.Failure("FailedToRemoveSharing", "Failed to remove home sharing.");
}