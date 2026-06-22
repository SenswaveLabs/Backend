using Senswave.Homes.Application.Rooms.Features.UpdateRoom;

namespace Senswave.Homes.Api.Rooms.UpdateRoom;

internal static class UpdateRoomExtensions
{
    internal static UpdateRoomCommand ToCommand(this UpdateRoomRequest dto, Guid sessionUserId, Guid roomId) => new()
    {
        Name = dto.Name,
        RoomId = roomId,
        UserId = sessionUserId
    };
}
