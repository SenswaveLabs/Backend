namespace Senswave.Homes.Application.Rooms.Features.DisplayRooms;

internal class DisplayRoomsErrors
{
    public static Error RoomsNotFoundInHome = Error.NotFound("RoomsNotFoundInHome", "Rooms were not found in home.");

    public static Error AccessDeniedForUser = Error.NotFound("HomeNotFound", "Home was not found for user.");
}
