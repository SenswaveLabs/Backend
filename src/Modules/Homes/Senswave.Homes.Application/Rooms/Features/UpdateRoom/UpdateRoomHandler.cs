using Senswave.Homes.Domain.Rooms.Repositories;
using Senswave.Homes.Domain.Rooms.Services;

namespace Senswave.Homes.Application.Rooms.Features.UpdateRoom;

public class UpdateRoomHandler(
    IRoomAccessService accessService,
    IRoomCommandRepository repository)
    : ICommandHandler<UpdateRoomCommand>
{
    public async Task<Result> Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
    {
        var hasAccess = await accessService.CanManage(request.UserId, request.RoomId, cancellationToken);

        if (!hasAccess)
            return Result.Failure(hasAccess.Errors);

        var room = await repository.GetRoom(request.RoomId, cancellationToken);

        if (room is null)
            return Result.Failure(UpdateRoomErrors.RoomNotFound);

        if (!string.IsNullOrWhiteSpace(request.Name))
            room.Name = request.Name;

        var update = await repository.UpdateRoom(room, cancellationToken);

        if (!update)
            return Result.Failure(update.Errors);

        return Result.Success();
    }
}
