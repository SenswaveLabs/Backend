namespace Senswave.Automations.Application.Features.GetAutomation;

public class GetAutomationQuery : IQuery<Models.AutomationModel>
{
    public Guid AutomationId { get; set; } = Guid.Empty;

    public Guid UserId { get; set; } = Guid.Empty;
}