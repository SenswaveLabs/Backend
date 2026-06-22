namespace Senswave.Devices.Application.Widgets.Features.State;

public class StateCommand : ICommand
{
    public Guid UserId { get; set; } = Guid.Empty;

    public Guid WidgetId { get; set; } = Guid.Empty;

    public bool Enabled { get; set; } = false;
}
