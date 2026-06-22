namespace Senswave.Homes.Domain.Rooms.Services;

public interface IRoomAccessService
{
    Task<Result> CanManage(Guid userId, Guid roomId, CancellationToken cancellationToken);
    Task<Result> CanManageHome(Guid userId, Guid homeId, CancellationToken cancellationToken);
    Task<Result> CanDisplayHome(Guid userId, Guid homeId, CancellationToken cancellationToken);
}
