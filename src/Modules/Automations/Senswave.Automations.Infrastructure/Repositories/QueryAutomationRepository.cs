using Microsoft.EntityFrameworkCore;
using Senswave.Automations.Domain.Entities;
using Senswave.Automations.Domain.Repositories;

namespace Senswave.Automations.Infrastructure.Repositories;

internal sealed class QueryAutomationRepository(AutomationsContext context) : IQueryAutomationRepository
{
    public Task<List<Automation>> GetAutomationsByHomeIdAndCondition(Guid homeId, Guid operationId, CancellationToken cancellationToken) => context.Automations
        .Include(a => a.HomesReference)
        .Where(a => a.HomesReference.HomeId == homeId)
        .Include(a => a.Conditions)
        .Include(a => a.Results)
        .Where(a => a.Conditions.Select(x => x.OperationId).Contains(operationId))
        .ToListAsync(cancellationToken);

    public Task<Automation?> GetAutomationByName(Guid homeId, string name, CancellationToken cancellationToken) => context.Automations
        .Include(x => x.HomesReference)
        .Where(x => x.HomesReference.HomeId == homeId && x.Name == name)
        .FirstOrDefaultAsync(cancellationToken);

    public Task<List<Automation>> GetAutomationByHomeId(Guid homeId, CancellationToken cancellationToken) => context.Automations
        .Include(x => x.HomesReference)
        .Where(x => x.HomesReference.HomeId == homeId)
        .Include(a => a.Conditions)
        .Include(a => a.Results)
        .ToListAsync(cancellationToken);

    public Task<Automation?> GetAutomation(Guid automationId, CancellationToken cancellationToken) => context.Automations
        .Where(x => x.Id == automationId)
        .Include(a => a.Conditions)
        .Include(a => a.Results)
        .Include(a => a.HomesReference)
        .AsNoTracking()
        .FirstOrDefaultAsync(cancellationToken);

    public Task<int> CountAutomationsByHome(Guid homeId, CancellationToken cancellationToken) => context.Automations
        .Include(x => x.HomesReference)
        .Where(x => x.HomesReference.HomeId == homeId)
        .CountAsync(cancellationToken);

    public async Task<HomeReference?> GetHomeReference(Guid homeId, CancellationToken cancellationToken)
    {
        return await context.HomeReferences
            .AsNoTracking()
            .FirstOrDefaultAsync(hr => hr.HomeId == homeId, cancellationToken);
    }
}
