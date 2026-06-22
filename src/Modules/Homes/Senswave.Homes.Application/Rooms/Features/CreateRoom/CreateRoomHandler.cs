using Senswave.Homes.Domain;
using Senswave.Homes.Domain.Rooms.Entities;
using Senswave.Homes.Domain.Rooms.Repositories;
using Senswave.Homes.Domain.Rooms.Services;

namespace Senswave.Homes.Application.Rooms.Features.CreateRoom;

public sealed class CreateRoomHandler(
    IOptions<HomeModuleOptions> options,
    IRoomAccessService accessService,
    IRoomQueryRepository queryRepository,
    IRoomCommandRepository commandRespository) : ICommandHandler<CreateRoomCommand, Room>
{
    public async Task<Result<Room>> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        var canManage = await accessService.CanManageHome(request.UserId, request.HomeId, cancellationToken);

        if (!canManage)
            return Result<Room>.Failure(canManage.Errors);

        var roomExists = await queryRepository.RoomExists(request.HomeId, request.Name, cancellationToken);

        if (roomExists)
            return Result<Room>.Failure([CreateRoomError.RoomAlreadyExists]);

        //TODO: Redis Lock per User

        var currentRooms = await queryRepository.CountRoomsByHome(request.HomeId, cancellationToken);

        if (options.Value.Limits.RoomsPerHome <= currentRooms)
            return Result<Room>.Failure(CreateRoomError.LimitOfRoomsReached);

        var room = new Room
        {
            HomeId = request.HomeId,
            Name = request.Name,
        };

        var result = await commandRespository.CreateRoom(room, cancellationToken);

        if (!result)
            return Result<Room>.Failure(result.Errors);

        return Result<Room>.Success(room);
    }
}
