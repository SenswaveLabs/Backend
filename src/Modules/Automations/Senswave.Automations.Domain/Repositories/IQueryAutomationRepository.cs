using Senswave.Automations.Domain.Entities;

namespace Senswave.Automations.Domain.Repositories;

public interface IQueryAutomationRepository
{
    Task<List<Automation>> GetAutomationsByHomeIdAndCondition(Guid homeId, Guid operationId, CancellationToken cancellationToken);

    Task<Automation?> GetAutomationByName(Guid homeId, string name, CancellationToken cancellationToken);

    Task<List<Automation>> GetAutomationByHomeId(Guid homeId, CancellationToken cancellationToken);

    Task<Automation?> GetAutomation(Guid automationId, CancellationToken cancellationToken);

    Task<int> CountAutomationsByHome(Guid homeId, CancellationToken cancellationToken);

    Task<HomeReference?> GetHomeReference(Guid homeId, CancellationToken cancellationToken);
}
