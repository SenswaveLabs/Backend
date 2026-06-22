using System.Text.Json.Nodes;
using Senswave.Devices.Domain.Devices.Enums;

namespace Senswave.Devices.Application.Devices.Features.UpdateDevice;

public class UpdateDeviceCommand : ICommand
{
    public Guid UserId { get; set; } = Guid.Empty;
    public Guid DeviceId { get; set; } = Guid.Empty;
    public Guid? RoomId { get; set; } = Guid.Empty;

    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;

    public Guid? TileOperationId { get; set; } = null;
    public Guid? TileDisplayableOperationId { get; set; } = null;
    public JsonObject? TileConfiguration { get; set; } = null;
    public DeviceTileType TileType { get; set; } = DeviceTileType.Empty;

    public Guid PresenceOperationId { get; set; } = Guid.Empty;
    public DevicePresenceType PresenceType { get; set; } = DevicePresenceType.Empty;
}