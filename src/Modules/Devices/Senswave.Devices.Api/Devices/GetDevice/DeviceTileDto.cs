using System.Text.Json.Nodes;

namespace Senswave.Devices.Api.Devices.GetDevice;

public class DeviceTileDto
{
    public string Type { get; set; } = string.Empty;

    public string? OperationId { get; set; }

    public string? DisplayableOperationId { get; set; }

    public JsonObject? Configuration { get; set; }
}
