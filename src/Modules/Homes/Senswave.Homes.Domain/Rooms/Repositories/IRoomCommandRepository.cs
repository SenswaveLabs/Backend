using Senswave.Homes.Domain.Rooms.Entities;

namespace Senswave.Homes.Domain.Rooms.Repositories;

public interface IRoomCommandRepository
{
    Task<Result> CreateRoom(Room room, CancellationToken cancellationToken);

    Task<Result> UpdateRoom(Room room, CancellationToken cancellationToken);

    Task<Result> DeleteRoom(Room room, CancellationToken cancellationToken);

    Task<Room?> GetRoom(Guid roomId, CancellationToken cancellationToken);
}
