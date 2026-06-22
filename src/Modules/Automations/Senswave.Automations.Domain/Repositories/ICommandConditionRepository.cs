using Senswave.Automations.Domain.Entities;

namespace Senswave.Automations.Domain.Repositories;

public interface ICommandConditionRepository
{
    public Task<AutomationCondition?> GetAutomationCondition(Guid conditionId, CancellationToken cancellationToken);
    public Task<Result> DeleteAutomationCondition(AutomationCondition automationCondition, CancellationToken cancellationToken);
    public Task<Result> AddAutomationCondition(AutomationCondition automationCondition, CancellationToken cancellationToken);
}