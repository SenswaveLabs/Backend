using Microsoft.EntityFrameworkCore;
using Senswave.Homes.Domain.Homes.Entities;
using Senswave.Homes.Domain.Homes.Repositories;
using Senswave.Homes.Domain.Sharings.Enums;

namespace Senswave.Homes.Infrastructure.Homes.Repositories;

internal sealed class HomeQueryRepository(HomesContext context) : IHomeQueryRepository
{
    public Task<bool> CanDisplayHome(Guid userId, Guid homeId, CancellationToken cancellationToken) => context.HomeSharings
        .Where(x => x.HomeId == homeId)
        .Where(x => x.UserId == userId && x.SharingType >= HomeSharingType.Display)
        .AsNoTracking()
        .AnyAsync(cancellationToken);

    public Task<bool> CanManageHome(Guid userId, Guid homeId, CancellationToken cancellationToken) => context.HomeSharings
        .Where(x => x.HomeId == homeId)
        .Where(x => x.UserId == userId && x.SharingType >= HomeSharingType.Manage)
        .AsNoTracking()
        .AnyAsync(cancellationToken);

    public Task<bool> IsHomeOwner(Guid userId, Guid homeId, CancellationToken cancellationToken) => context.Homes
        .Where(x => x.Id == homeId)
        .Where(x => x.OwnerId == userId)
        .AsNoTracking()
        .AnyAsync(cancellationToken);

    public Task<Home?> GetHome(Guid homeId, CancellationToken cancellationToken) => context.Homes
        .Where(x => x.Id == homeId)
        .Include(x => x.DataSourceReference)
        .AsNoTracking()
        .FirstOrDefaultAsync(cancellationToken);

    public Task<Home?> GetHomeWithSharedUsers(Guid homeId, CancellationToken cancellationToken) => context.Homes
        .Where(x => x.Id == homeId)
        .Include(x => x.HomeSharing)
        .Include(x => x.DataSourceReference)
        .AsNoTracking()
        .FirstOrDefaultAsync(cancellationToken);

    public Task<Home?> GetHomeWithRooms(Guid homeId, CancellationToken cancellationToken) => context.Homes
        .Where(x => x.Id == homeId)
        .Include(x => x.Rooms)
        .Include(x => x.DataSourceReference)
        .AsNoTracking()
        .FirstOrDefaultAsync(cancellationToken);

    public Task<bool> IsHomeShared(Guid userId, Guid homeId, CancellationToken cancellationToken) => context.HomeSharings
        .Where(x => x.HomeId == homeId)
        .Where(x => x.UserId == userId)
        .AsNoTracking()
        .AnyAsync(cancellationToken);

    public Task<bool> HasManageAccessToHome(Guid userId, Guid deviceId, CancellationToken cancellationToken) => context.HomeSharings
        .Where(x => x.HomeId == deviceId)
        .Where(x => x.UserId == userId && x.SharingType >= HomeSharingType.Manage)
        .AsNoTracking()
        .AnyAsync(cancellationToken);

    public Task<bool> HasDisplayAccessToHome(Guid userId, Guid deviceId, CancellationToken cancellationToken) => context.HomeSharings
        .Where(x => x.HomeId == deviceId)
        .Where(x => x.UserId == userId && x.SharingType >= HomeSharingType.Display)
        .AsNoTracking()
        .AnyAsync(cancellationToken);

    public Task<int> CountOwnedHomesByUser(Guid userId, CancellationToken cancellationToken) => context.Homes
        .Where(x => x.OwnerId == userId)
        .AsNoTracking()
        .CountAsync(cancellationToken);

    public Task<List<Home>> GetHomes(Guid userId, int skip, int size, CancellationToken cancellationToken) => context.Homes
        .Where(home => home.OwnerId == userId)
        .OrderBy(home => home.Name)
        .Skip(skip)
        .Take(size)
        .Include(home => home.Location)
        .Include(x => x.DataSourceReference)
        .AsNoTracking()
        .ToListAsync(cancellationToken);
}
