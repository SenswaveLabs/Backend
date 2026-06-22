namespace Senswave.Homes.Domain.Homes.Services;

public interface IHomeAccessService
{
    Task<Result> IsOwner(Guid userId, Guid homeId, CancellationToken cancellationToken);
    Task<Result> CanDisplay(Guid userId, Guid homeId, CancellationToken cancellationToken);
    Task<Result> CanManage(Guid userId, Guid homeId, CancellationToken cancellationToken);
}
