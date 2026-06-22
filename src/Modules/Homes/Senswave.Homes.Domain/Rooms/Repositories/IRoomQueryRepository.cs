using Senswave.Homes.Domain.Rooms.Entities;

namespace Senswave.Homes.Domain.Rooms.Repositories;

public interface IRoomQueryRepository
{
    Task<Guid> GetHomeIdByRoomId(Guid roomId, CancellationToken cancellationToken);

    Task<Room?> GetRoom(Guid roomId, CancellationToken cancellationToken);

    Task<List<Room>> GetRooms(Guid homeId, CancellationToken cancellationToken);

    Task<bool> RoomExistsInHome(Guid roomId, Guid homeId, CancellationToken cancellationToken);

    Task<bool> RoomExists(Guid id, string name, CancellationToken cancellationToken);

    Task<int> CountRoomsByHome(Guid homeId, CancellationToken cancellationToken);
}
