namespace Senswave.Homes.Application.Rooms.Features.UpdateRoom;

internal class UpdateRoomErrors
{
    public static Error RoomNotFound => Error.NotFound("RoomNotFound", "Room not found.");
}
