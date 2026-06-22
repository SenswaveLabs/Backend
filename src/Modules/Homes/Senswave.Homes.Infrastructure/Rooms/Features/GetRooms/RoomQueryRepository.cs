using Microsoft.EntityFrameworkCore;
using Senswave.Homes.Domain.Rooms.Entities;
using Senswave.Homes.Domain.Rooms.Repositories;

namespace Senswave.Homes.Infrastructure.Rooms.Features.GetRooms;

internal sealed class RoomQueryRepository(HomesContext context) : IRoomQueryRepository
{
    public Task<Guid> GetHomeIdByRoomId(Guid roomId, CancellationToken cancellationToken) => context.Rooms
        .Where(x => x.Id == roomId)
        .AsNoTracking()
        .Select(x => x.Home.Id)
        .FirstOrDefaultAsync(cancellationToken);

    public Task<List<Room>> GetRooms(Guid homeId, CancellationToken cancellationToken) => context.Rooms
        .Where(x => x.Home.Id == homeId)
        .AsNoTracking()
        .ToListAsync(cancellationToken);

    public Task<bool> RoomExistsInHome(Guid roomId, Guid homeId, CancellationToken cancellationToken) => context.Rooms
        .Where(r => r.Id == roomId)
        .Where(r => r.Home.Id == homeId)
        .AsNoTracking()
        .AnyAsync(cancellationToken);

    public Task<bool> RoomExists(Guid homeId, string name, CancellationToken cancellationToken) => context.Rooms
        .Where(r => r.HomeId == homeId)
        .Where(r => r.Name == name)
        .AsNoTracking()
        .AnyAsync(cancellationToken);

    public Task<int> CountRoomsByHome(Guid homeId, CancellationToken cancellationToken) => context.Rooms
        .Where(r => r.HomeId == homeId)
        .AsNoTracking()
        .CountAsync(cancellationToken);

    public Task<Room?> GetRoom(Guid roomId, CancellationToken cancellationToken) => context.Rooms
        .Where(r => r.Id == roomId)
        .AsNoTracking()
        .FirstOrDefaultAsync(cancellationToken);
}
