namespace Senswave.Integration.DataTransfer.Devices;

public class WidgetActionEvent
{
    public Guid DeviceId { get; set; }
    public List<Guid> WidgetIds { get; set; } = [];
}
