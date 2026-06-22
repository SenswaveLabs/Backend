using System.Text.Json.Nodes;

namespace Senswave.Devices.Api.Widgets.Models.DisplayWidget;

internal class DisplayWidgetDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public bool Enabled { get; set; }

    public JsonObject Display { get; set; } = [];

    public DateTime UpdatedAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
