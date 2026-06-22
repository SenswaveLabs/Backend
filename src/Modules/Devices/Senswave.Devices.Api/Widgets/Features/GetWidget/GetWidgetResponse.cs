using System.Text.Json.Nodes;

namespace Senswave.Devices.Api.Widgets.Features.GetWidget;

public class GetWidgetResponse
{
    public Guid Id { get; set; }

    public Guid DeviceId { get; set; }

    public Guid OperationId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public bool Enabled { get; set; }

    public JsonObject Configuration { get; set; } = [];
}