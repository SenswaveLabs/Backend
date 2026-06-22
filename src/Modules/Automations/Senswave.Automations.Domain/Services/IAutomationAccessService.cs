using Senswave.Automations.Domain.Entities;

namespace Senswave.Automations.Domain.Services;

public interface IAutomationAccessService
{
    public bool IsOwner(HomeReference homeReference, Guid userId);
    public Task<Result> CanDisplayHome(HomeReference homeReference, Guid userId, CancellationToken cancellationToken);

    public Task<Result> CanManageHome(HomeReference homeReference, Guid userId, CancellationToken cancellationToken);

    public Task<Result> CanActDevices(IList<Guid> operationIds, Guid userId, CancellationToken cancellationToken);

}