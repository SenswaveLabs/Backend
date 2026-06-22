using Senswave.Homes.Application.Rooms.Features.DeleteRoom;

namespace Senswave.Homes.Api.Rooms.DeleteRoom;

public static class DeleteRoomExtensions
{
    public static DeleteRoomCommand ToDeleteRoomCommand(this Guid roomId, Guid homeId, IRequestContext context) => new()
    {
        UserId = context.UserId,
        RoomId = roomId,
        HomeId = homeId
    };
}
