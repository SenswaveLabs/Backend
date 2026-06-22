using Senswave.Homes.Domain.Rooms.Entities;

namespace Senswave.Homes.Application.Rooms.Features.DisplayRooms;

public class DisplayRoomsQuery : IQuery<List<Room>>
{
    public Guid UserId { get; set; } = Guid.Empty;
    public Guid HomeId { get; set; } = Guid.Empty;
}
