namespace Senswave.Homes.Application.Rooms.Features.DeleteRoom;

internal class DeleteRoomErrors
{
    public static readonly Error RoomNotFound = Error.NotFound("RoomNotFound", "The specified room does not exist in the home.");
}
