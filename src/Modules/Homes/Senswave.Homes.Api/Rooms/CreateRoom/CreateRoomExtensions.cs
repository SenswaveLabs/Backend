using Senswave.Abstractions.Resulting;
using Senswave.Homes.Application.Rooms.Features.CreateRoom;
using Senswave.Homes.Domain.Rooms.Entities;

namespace Senswave.Homes.Api.Rooms.CreateRoom;

internal static class CreateRoomExtensions
{
    internal static CreateRoomCommand ToCommand(this CreateRoomRequest dto, Guid sessionUserId, Guid homeId) => new()
    {
        HomeId = homeId,
        Name = dto.Name,
        UserId = sessionUserId
    };

    internal static RoomCreatedResponse ToCreatedResponse(this Result<Room> result) => new()
    {
        Id = result.Data.Id
    };
}
