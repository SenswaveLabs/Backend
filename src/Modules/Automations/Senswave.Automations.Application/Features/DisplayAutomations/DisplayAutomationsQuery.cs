namespace Senswave.Automations.Application.Features.DisplayAutomations;

public class DisplayAutomationsQuery : IQuery<IList<Models.AutomationModel>>
{
    public Guid UserId { get; set; } = Guid.Empty;

    public Guid HomeId { get; set; } = Guid.Empty;
}