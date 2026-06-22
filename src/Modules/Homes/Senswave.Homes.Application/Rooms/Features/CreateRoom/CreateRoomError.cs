namespace Senswave.Homes.Application.Rooms.Features.CreateRoom;

internal class CreateRoomError
{
    public static Error LimitOfRoomsReached
        => Error.Conflict("LimitOfRoomsReached", "The maximum number of rooms in this home has been reached.");

    public static Error RoomAlreadyExists
        => Error.Conflict("RoomAlreadyUsed", "Room already exists in home");
}
