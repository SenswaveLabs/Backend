using Senswave.Homes.Domain.Rooms.Entities;
using Senswave.Homes.Domain.Rooms.Repositories;
using Senswave.Homes.Domain.Rooms.Services;

namespace Senswave.Homes.Application.Rooms.Features.GetRoom;

internal class GetRoomHandler(
    IRoomAccessService accessService,
    IRoomQueryRepository repository,
    ILogger<GetRoomHandler> logger) : IQueryHandler<GetRoomQuery, Room>
{
    public async Task<Result<Room>> Handle(GetRoomQuery request, CancellationToken cancellationToken)
    {
        var hasAccess = await accessService.CanDisplayHome(request.UserId, request.HomeId, cancellationToken);

        if (!hasAccess)
        {
            logger.LogError("[Room: {roomId}] No didsplay access to room.", request.RoomId);
            return Result<Room>.Failure(hasAccess.Errors);
        }

        var room = await repository.GetRoom(request.RoomId, cancellationToken);

        if (room is null)
        {
            logger.LogError("[Room: {roomId}] Room not found.", request.RoomId);
            return Result<Room>.Failure(GetRoomErrors.RoomNotFound);
        }

        return Result<Room>.Success(room!);
    }
}
