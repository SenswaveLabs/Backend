
using Senswave.Homes.Domain.Homes.Entities;

namespace Senswave.Homes.Domain.Homes.Repositories;

public interface IHomeQueryRepository
{
    Task<bool> CanDisplayHome(Guid userId, Guid homeId, CancellationToken cancellationToken);
    Task<bool> CanManageHome(Guid userId, Guid homeId, CancellationToken cancellationToken);
    Task<bool> IsHomeOwner(Guid userId, Guid homeId, CancellationToken cancellationToken);

    Task<List<Home>> GetHomes(Guid userId, int skip, int size, CancellationToken cancellationToken);
    Task<Home?> GetHomeWithSharedUsers(Guid homeId, CancellationToken cancellationToken);
    Task<Home?> GetHome(Guid homeId, CancellationToken cancellationToken);
    Task<Home?> GetHomeWithRooms(Guid homeId, CancellationToken cancellationToken);
    Task<bool> IsHomeShared(Guid userId, Guid homeId, CancellationToken cancellationToken);
    Task<bool> HasManageAccessToHome(Guid userId, Guid deviceId, CancellationToken cancellationToken);
    Task<bool> HasDisplayAccessToHome(Guid userId, Guid deviceId, CancellationToken cancellationToken);
    Task<int> CountOwnedHomesByUser(Guid userId, CancellationToken cancellationToken);
}
