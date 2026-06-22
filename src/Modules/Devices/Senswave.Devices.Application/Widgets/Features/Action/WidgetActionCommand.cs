using System.Text.Json.Nodes;

namespace Senswave.Devices.Application.Widgets.Features.Action;

public class WidgetActionCommand : ICommand
{
    public Guid WidgetId { get; set; }
    public Guid UserId { get; set; }
    public JsonValue Value { get; set; } = JsonValue.Create("");
}
