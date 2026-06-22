using System.Text.Json.Nodes;

namespace Senswave.Devices.Api.Widgets.Features.CreateWidget;

public class CreateWidgetRequest
{
    public Guid OperationId { get; set; } = Guid.Empty;

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public JsonObject Configuration { get; set; } = [];
}