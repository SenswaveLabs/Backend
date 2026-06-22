using Senswave.Automations.Domain.Entities;

namespace Senswave.Automations.Domain.Repositories;

public interface ICommandResultRepository
{
    public Task<AutomationResult?> GetAutomationResult(Guid resultId, CancellationToken cancellationToken);
    public Task<Result> DeleteResult(AutomationResult result, CancellationToken cancellationToken);

    public Task<Result> AddResult(AutomationResult result, CancellationToken cancellationToken);
}