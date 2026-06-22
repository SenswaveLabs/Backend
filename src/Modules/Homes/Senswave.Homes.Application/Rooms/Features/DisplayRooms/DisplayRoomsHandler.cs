using Senswave.Homes.Domain.Rooms.Entities;
using Senswave.Homes.Domain.Rooms.Repositories;
using Senswave.Homes.Domain.Rooms.Services;

namespace Senswave.Homes.Application.Rooms.Features.DisplayRooms;

internal sealed class DisplayRoomsHandler(
    IRoomAccessService accessService,
    IRoomQueryRepository repository) : IQueryHandler<DisplayRoomsQuery, List<Room>>
{
    public async Task<Result<List<Room>>> Handle(DisplayRoomsQuery request, CancellationToken cancellationToken)
    {
        var hasAccess = await accessService.CanDisplayHome(request.UserId, request.HomeId, cancellationToken);

        if (!hasAccess)
            return Result<List<Room>>.Failure(DisplayRoomsErrors.AccessDeniedForUser, hasAccess.Errors);

        var roomsInHome = await repository.GetRooms(request.HomeId, cancellationToken);

        if (roomsInHome.Count == 0)
            return Result<List<Room>>.Failure([DisplayRoomsErrors.RoomsNotFoundInHome]);

        return Result<List<Room>>.Success(roomsInHome);
    }
}
