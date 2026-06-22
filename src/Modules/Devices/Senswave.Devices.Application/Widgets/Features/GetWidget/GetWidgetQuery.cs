using Senswave.Devices.Domain.Widgets.Entities;

namespace Senswave.Devices.Application.Widgets.Features.GetWidget;

public class GetWidgetQuery : IQuery<Widget>
{
    public Guid UserId { get; set; }
    public Guid WidgetId { get; set; }
}