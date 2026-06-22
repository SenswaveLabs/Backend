using Senswave.Automations.Domain.Repositories;
using Senswave.Automations.Domain.Services;

namespace Senswave.Automations.Application.Features.DeleteResult;

public class DeleteResultHandler(
    IAutomationAccessService automationAccessService,
    ICommandResultRepository commandResultRepository)
    : ICommandHandler<DeleteResultCommand>
{
    public async Task<Result> Handle(DeleteResultCommand request, CancellationToken cancellationToken)
    {
        // Only owner of the home can delete result from automation
        var automationResult = await commandResultRepository.GetAutomationResult(request.ResultId, cancellationToken);

        if (automationResult is null)
            return Result.Failure([DeleteResultErrors.ResultNotFound]);

        var isOwner = automationAccessService.IsOwner(automationResult.Automation.HomesReference, request.UserId);

        if (!isOwner)
            return Result.Failure([DeleteResultErrors.UserHasNoAccess]);

        // User can not delete AutomationResult if there is only one AutomationResult in Automation
        if (automationResult.Automation.Results.Count == 1)
            return Result.Failure([DeleteResultErrors.CanNotDeleteResultIfThereIsOnlyResultInAutomation]);

        var transactionResult = await commandResultRepository.DeleteResult(automationResult, cancellationToken);

        if (transactionResult.IsFailure)
            return Result.Failure([DeleteResultErrors.CanNotDeleteResult]);

        return Result.Success();
    }
}