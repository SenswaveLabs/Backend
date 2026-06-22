using System.Text.Json.Nodes;

namespace Senswave.Devices.Api.Operations.GetOperation;

public class GetOperationResponse
{
    public Guid Id { get; set; } = Guid.Empty;

    public string Topic { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public JsonObject Configuration { get; set; } = [];
}