using Senswave.Homes.Domain.Rooms.Entities;

namespace Senswave.Homes.Application.Rooms.Features.CreateRoom;

public class CreateRoomCommand : ICommand<Room>
{
    public Guid UserId { get; set; } = Guid.Empty;
    public Guid HomeId { get; set; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;
}
