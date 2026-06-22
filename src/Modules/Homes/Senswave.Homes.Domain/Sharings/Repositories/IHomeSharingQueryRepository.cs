using Senswave.Homes.Domain.Sharings.Entities;
using Senswave.Homes.Domain.Sharings.Enums;

namespace Senswave.Homes.Domain.Sharings.Repositories;

public interface IHomeSharingQueryRepository
{
    Task<bool> UserCanReadHome(Guid userId, Guid homeId, HomeSharingType lowestHomeSharingType, CancellationToken cancellationToken);

    Task<bool> HomeHasDataSource(Guid homeId, CancellationToken cancellationToken);

    Task<HomeSharingInvitation?> GetInvitation(Guid friendId, Guid homeId, CancellationToken cancellationToken);

    Task<List<HomeSharing>> GetSharingUsers(Guid homeId, CancellationToken cancellationToken);

    Task<List<HomeSharing>> GetAllHomeSharingToUser(Guid userId, HomeSharingType privilege, CancellationToken cancellationToken);
    Task<int> CountUsersByHome(Guid homeId, CancellationToken cancellationToken);
}
