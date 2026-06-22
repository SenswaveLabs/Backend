namespace Senswave.Homes.Application.Rooms.Features.DeleteRoom;

public class DeleteRoomCommand : ICommand
{
    public Guid UserId { get; init; } = Guid.Empty;
    public Guid HomeId { get; init; } = Guid.Empty;
    public Guid RoomId { get; init; } = Guid.Empty;
}
