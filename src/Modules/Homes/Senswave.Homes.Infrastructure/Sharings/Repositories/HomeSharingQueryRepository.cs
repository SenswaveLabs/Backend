using Microsoft.EntityFrameworkCore;
using Senswave.Homes.Domain.Sharings.Entities;
using Senswave.Homes.Domain.Sharings.Enums;
using Senswave.Homes.Domain.Sharings.Repositories;

namespace Senswave.Homes.Infrastructure.Sharings.Repositories;

internal class HomeSharingQueryRepository(HomesContext context) : IHomeSharingQueryRepository
{
    public async Task<bool> UserCanReadHome(Guid userId, Guid homeId, HomeSharingType lowestHomeSharingType, CancellationToken cancellationToken)
    {
        var isOwner = await context.Homes
            .Where(x => x.Id == homeId)
            .Where(x => x.OwnerId == userId)
            .AsNoTracking()
            .AnyAsync(cancellationToken);

        if (isOwner)
            return true;

        var hasGotPrivileges = await context.HomeSharings
            .Include(x => x.Home)
            .Where(x => x.Home.Id == homeId && x.UserId == userId && x.SharingType >= lowestHomeSharingType)
            .AsNoTracking()
            .AnyAsync(cancellationToken);

        return hasGotPrivileges;
    }

    public Task<bool> HomeHasDataSource(Guid homeId, CancellationToken cancellationToken) => context.Homes
        .Where(x => x.Id == homeId)
        .Where(x => x.DataSourceReference != null)
        .AsNoTracking()
        .AnyAsync(cancellationToken);

    public Task<HomeSharingInvitation?> GetInvitation(Guid friendId, Guid homeId, CancellationToken cancellationToken) => context.HomeInvitations
        .Where(x => x.HomeId == homeId)
        .Where(x => x.FriendId == friendId)
        .AsNoTracking()
        .FirstOrDefaultAsync(cancellationToken);

    public Task<List<HomeSharing>> GetSharingUsers(Guid homeId, CancellationToken cancellationToken) => context.HomeSharings
        .Where(x => x.Home.Id == homeId)
        .AsNoTracking()
        .ToListAsync(cancellationToken);

    public Task<List<HomeSharing>> GetAllHomeSharingToUser(Guid userId, HomeSharingType privilege, CancellationToken cancellationToken) => context.HomeSharings
        .Where(x => x.UserId == userId)
        .Where(x => x.SharingType >= privilege)
        .Include(x => x.Home)
        .AsNoTracking()
        .ToListAsync(cancellationToken);

    public Task<int> CountUsersByHome(Guid homeId, CancellationToken cancellationToken) => context.HomeSharings
        .Where(x => x.Home.Id == homeId)
        .AsNoTracking()
        .CountAsync(cancellationToken);
}
