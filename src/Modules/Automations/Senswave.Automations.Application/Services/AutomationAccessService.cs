using Senswave.Automations.Domain.Entities;
using Senswave.Automations.Domain.Services;
using Senswave.Integration.Devices.ActionAccessToOperations;
using Senswave.Integration.Homes.CanDisplayHome;
using Senswave.Integration.Homes.CanManageHome;

namespace Senswave.Automations.Application.Services;

public class AutomationAccessService(
    IRequestClient<CanDisplayHomeRequest> displayHomeClient,
    IRequestClient<CanManageHomeRequest> manageHomeClient,
    IRequestClient<ActionAccessToOperationsRequest> operationAccessClient) : IAutomationAccessService
{
    #region Error

    private static readonly Error AccessDenied = Error.Failure("AccessDenied", "Access denied or element not found.");

    #endregion

    public bool IsOwner(HomeReference homeReference, Guid userId)
    {
        return homeReference.OwnerId == userId;
    }

    public async Task<Result> CanDisplayHome(HomeReference homeReference, Guid userId, CancellationToken cancellationToken)
    {
        if (IsOwner(homeReference, userId))
            return Result.Success();

        var request = new CanDisplayHomeRequest
        {
            UserId = userId,
            HomeId = homeReference.HomeId
        };

        var result = await displayHomeClient.GetResponse<CanDisplayHomeResponse>(request, cancellationToken);

        if (result.Message.IsSuccess)
            return Result.Success();

        return Result.Failure(AccessDenied);
    }

    public async Task<Result> CanManageHome(HomeReference homeReference, Guid userId, CancellationToken cancellationToken)
    {
        if (IsOwner(homeReference, userId))
            return Result.Success();

        var request = new CanManageHomeRequest
        {
            UserId = userId,
            HomeId = homeReference.HomeId
        };

        var result = await manageHomeClient.GetResponse<CanManageHomeResponse>(request, cancellationToken);

        if (result.Message.IsSuccess)
            return Result.Success();

        return Result.Failure(AccessDenied);
    }

    public async Task<Result> CanActDevices(IList<Guid> operationIds, Guid userId, CancellationToken cancellationToken)
    {
        var request = new ActionAccessToOperationsRequest
        {
            UserId = userId,
            OperationIds = operationIds.ToHashSet()
        };

        var access = await operationAccessClient.GetResponse<ActionAccessToOperationsResponse>(request, cancellationToken);

        if (access.Message.IsSuccess)
            return Result.Success();

        return Result.Failure(AccessDenied);
    }
}