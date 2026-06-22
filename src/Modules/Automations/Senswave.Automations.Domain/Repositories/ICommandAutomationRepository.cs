using Senswave.Automations.Domain.Entities;

namespace Senswave.Automations.Domain.Repositories;

public interface ICommandAutomationRepository
{
    Task<Automation?> GetAutomation(Guid automationId, CancellationToken cancellationToken);
    Task<Automation?> GetAutomationToDelete(Guid automationId, CancellationToken cancellationToken);
    Task<Result> CreateAutomation(Automation automation, CancellationToken cancellationToken);
    Task<Result> UpdateAutomation(Automation automation, CancellationToken cancellationToken);
    Task<Result> DeleteAutomation(Automation automation, CancellationToken cancellationToken);

    Task<HomeReference?> GetHomeReference(Guid homeId, Guid userId, CancellationToken cancellationToken);
    Task<Result> CreateHomeReference(HomeReference homeReference, CancellationToken cancellationToken);
    Task<Result> DeleteHomeReference(HomeReference homeReference, CancellationToken cancellationToken);
}
