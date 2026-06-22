
using Senswave.Homes.Domain.Rooms.Entities;
using Senswave.Homes.Domain.Rooms.Repositories;
using Senswave.Homes.Domain.Rooms.Services;

namespace Senswave.Homes.Application.Rooms.Features.DeleteRoom;

public class DeleteRoomHandler(
    IRoomAccessService accessService,
    IRoomCommandRepository repository,
    ILogger<DeleteRoomHandler> logger)
    : ICommandHandler<DeleteRoomCommand>
{
    public async Task<Result> Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
    {
        var canManage = await accessService.CanManageHome(request.UserId, request.HomeId, cancellationToken);

        if (!canManage)
        {
            logger.LogWarning("[Delete Room] User {userId} has no access to home {homeId}.", request.UserId, request.HomeId);
            return Result<Room>.Failure(canManage.Errors);
        }

        var room = await repository.GetRoom(request.RoomId, cancellationToken);

        if (room is null)
        {
            logger.LogWarning("[Delete Room] Room {roomId} not found in home {homeId}.", request.RoomId, request.HomeId);
            return Result.Failure(DeleteRoomErrors.RoomNotFound);
        }

        var result = await repository.DeleteRoom(room, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogError("[Delete Room] Failed to delete room {roomId} in home {homeId}.", request.RoomId, request.HomeId);
            return Result.Failure(result.Errors);
        }

        return Result.Success();
    }
}
