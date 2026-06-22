using Senswave.Abstractions.Resulting;
using Senswave.Homes.Domain.Rooms.Entities;

namespace Senswave.Homes.Api.Rooms.DisplayRooms;

internal static class DisplayRoomsExtensions
{
    internal static DisplayRoomsResponse ToResponse(this Result<List<Room>> result) => new()
    {
        Items = result.Data.Select(x => new RoomDto
        {
            Id = x.Id,
            Name = x.Name
        }).ToList()
    };
}
