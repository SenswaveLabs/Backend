namespace Senswave.Devices.Domain.Widgets.Models;

public class WidgetMessageModel
{
    public Guid OperationId { get; set; }

    public JsonValue Value { get; set; } = JsonValue.Create(string.Empty);
}
