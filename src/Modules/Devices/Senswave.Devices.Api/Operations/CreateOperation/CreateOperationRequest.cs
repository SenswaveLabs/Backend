using System.Text.Json.Nodes;

namespace Senswave.Devices.Api.Operations.CreateOperation;

public class CreateOperationRequest
{
    public Guid DeviceId { get; set; } = Guid.Empty;

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public JsonObject Configuration { get; set; } = [];

    public string Topic { get; set; } = string.Empty;
}