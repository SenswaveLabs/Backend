using Senswave.Automations.Domain.Entities;

namespace Senswave.Automations.Application.Features.PutResultToAutomation;

public class PutResultCommand : ICommand
{
    public Guid UserId { get; set; } = Guid.Empty;
    public Guid AutomationId { get; set; } = Guid.Empty;

    public AutomationResult? Result { get; set; }
}