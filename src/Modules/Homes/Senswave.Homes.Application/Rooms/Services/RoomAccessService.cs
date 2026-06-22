using Senswave.Homes.Domain.Homes.Services;
using Senswave.Homes.Domain.Rooms.Repositories;
using Senswave.Homes.Domain.Rooms.Services;

namespace Senswave.Homes.Application.Rooms.Services;

public class RoomAccessService(
    IHomeAccessService accessService,
    IRoomQueryRepository queryRepository,
    ILogger<RoomAccessService> logger) : IRoomAccessService
{
    #region Errors

    private readonly Error HomeNotFound = Error.Failure("HomeNotFound", "Home not found.");

    #endregion

    public async Task<Result> CanManage(Guid userId, Guid roomId, CancellationToken cancellationToken)
    {
        var homeId = await queryRepository.GetHomeIdByRoomId(roomId, cancellationToken);

        if (homeId == default)
        {
            logger.LogWarning("[Room: {roomId}] Home not found for room.", roomId);
            return Result.Failure(HomeNotFound);
        }

        return await accessService.CanManage(userId, homeId, cancellationToken);
    }

    public Task<Result> CanManageHome(Guid userId, Guid homeId, CancellationToken cancellationToken)
        => accessService.CanManage(userId, homeId, cancellationToken);

    public Task<Result> CanDisplayHome(Guid userId, Guid homeId, CancellationToken cancellationToken)
        => accessService.CanDisplay(userId, homeId, cancellationToken);
}
