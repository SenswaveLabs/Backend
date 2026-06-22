using Refit;
using Senswave.Homes.Api.Rooms.CreateRoom;

namespace Senswave.Presentation.Seed.Homes.Clients;

public interface IRoomClient
{
    [Post("/v1/homes/{homeId}/rooms")]
    Task<RoomCreatedResponse> CreateRoom([Authorize(scheme: "Bearer")] string token, Guid homeId, [Body] CreateRoomRequest request);
}
