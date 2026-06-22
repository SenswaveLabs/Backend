using Senswave.Homes.Domain.Rooms.Entities;

namespace Senswave.Homes.Application.Rooms.Features.GetRoom;

public class GetRoomQuery : IQuery<Room>
{
    public Guid UserId { get; set; } = Guid.Empty;
    public Guid HomeId { get; set; } = Guid.Empty;
    public Guid RoomId { get; set; } = Guid.Empty;
}
