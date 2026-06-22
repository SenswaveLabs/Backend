namespace Senswave.Automations.Application.Features.DeleteAutomation;

public class DeleteAutomationCommand : ICommand
{
    public Guid AutomationId { get; set; }

    public Guid UserId { get; set; }
}