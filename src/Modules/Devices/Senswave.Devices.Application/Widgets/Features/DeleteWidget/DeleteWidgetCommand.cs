namespace Senswave.Devices.Application.Widgets.Features.DeleteWidget;

public class DeleteWidgetCommand : ICommand
{
    public Guid OwnerId { get; set; } = Guid.Empty;
    public Guid WidgetId { get; set; } = Guid.Empty;
}