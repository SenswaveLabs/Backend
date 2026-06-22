using Senswave.Homes.Domain.Rooms.Entities;

namespace Senswave.Homes.Api.Rooms.GetRoom;

internal static class GetRoomExtensions
{
    public static GetRoomResponse ToResponse(this Room room) => new()
    {
        Id = room.Id,
        Name = room.Name,
    };
}
