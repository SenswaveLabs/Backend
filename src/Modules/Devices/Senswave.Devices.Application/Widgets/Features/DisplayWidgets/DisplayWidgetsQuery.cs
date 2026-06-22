namespace Senswave.Devices.Application.Widgets.Features.DisplayWidgets;

public class DisplayWidgetsQuery : IQuery<List<DisplayWidgetsGroupModel>>
{
    public Guid DeviceId { get; set; }
    public Guid UserId { get; set; }
}
