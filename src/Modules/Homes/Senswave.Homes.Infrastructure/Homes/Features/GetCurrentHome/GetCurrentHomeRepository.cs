using Microsoft.EntityFrameworkCore;
using Senswave.Homes.Domain.Homes.Entities;
using Senswave.Homes.Domain.Homes.Features.GetCurrentHome;
using Senswave.Homes.Domain.Sharings.Enums;

namespace Senswave.Homes.Infrastructure.Homes.Features.GetCurrentHome;

internal sealed class GetCurrentHomeRepository(HomesContext context) : IGetCurrentHomeRepository
{
    public async Task<List<Home>> GetHomes(Guid userId, CancellationToken cancellationToken)
    {
        var userHomes = await UserHomes(userId, cancellationToken);
        var sharedHomes = await SharedHomes(userId, cancellationToken);

        return userHomes.Concat(sharedHomes)
            .DistinctBy(x => x.Id)
            .ToList();
    }

    private Task<List<Home>> UserHomes(Guid userId, CancellationToken cancellationToken)
        => context.Homes
        .Where(home => home.OwnerId == userId)
        .Include(x => x.Location)
        .AsNoTracking()
        .ToListAsync(cancellationToken);

    private Task<List<Home>> SharedHomes(Guid userId, CancellationToken cancellationToken)
        => context.HomeSharings
        .Where(x => x.UserId == userId && x.SharingType >= HomeSharingType.Display)
        .Include(x => x.Home)
        .Include(x => x.Home.Location)
        .Select(x => x.Home)
        .AsNoTracking()
        .ToListAsync(cancellationToken);
}

