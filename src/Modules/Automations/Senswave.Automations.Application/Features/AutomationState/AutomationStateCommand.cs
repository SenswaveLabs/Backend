namespace Senswave.Automations.Application.Features.AutomationState;

public class AutomationStateCommand : ICommand
{
    public Guid AutomationId { get; set; } = Guid.Empty;

    public Guid UserId { get; set; } = Guid.Empty;

    public bool IsEnabled { get; set; }
}