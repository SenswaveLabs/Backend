namespace Senswave.Homes.Application.Rooms.Features.UpdateRoom;

public class UpdateRoomCommand : ICommand
{
    public string Name { get; set; } = string.Empty;
    public Guid RoomId { get; set; } = Guid.Empty;
    public Guid UserId { get; set; } = Guid.Empty;
}
