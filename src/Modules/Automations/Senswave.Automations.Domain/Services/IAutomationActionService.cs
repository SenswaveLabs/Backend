using Senswave.Automations.Domain.Entities;

namespace Senswave.Automations.Domain.Services;

public interface IAutomationActionService
{
    public Task ProcessAutomationWithEvent(IList<Automation> automations, CancellationToken cancellationToken);
}