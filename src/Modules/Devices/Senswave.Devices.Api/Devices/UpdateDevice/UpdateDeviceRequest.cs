using System.Text.Json.Nodes;

namespace Senswave.Devices.Api.Devices.UpdateDevice;

public class UpdateDeviceRequest
{
    public Guid? RoomId { get; set; } = Guid.Empty;

    public string Name { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;


    public Guid? OperationId { get; set; } = null;
    public Guid? DisplayableOperationId { get; set; } = null;
    public JsonObject? Configuration { get; set; } = null;
    public string Type { get; set; } = string.Empty;

    public Guid PresenceOperationId { get; set; } = Guid.Empty;
    public string PresenceType { get; set; } = string.Empty;
}