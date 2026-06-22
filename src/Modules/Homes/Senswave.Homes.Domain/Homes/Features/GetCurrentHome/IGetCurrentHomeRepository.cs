using Senswave.Homes.Domain.Homes.Entities;

namespace Senswave.Homes.Domain.Homes.Features.GetCurrentHome;

public interface IGetCurrentHomeRepository
{
    Task<List<Home>> GetHomes(Guid userId, CancellationToken cancellationToken);
}
