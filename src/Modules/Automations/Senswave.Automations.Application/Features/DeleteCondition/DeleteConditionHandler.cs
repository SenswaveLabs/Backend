using Senswave.Automations.Domain.Repositories;
using Senswave.Automations.Domain.Services;

namespace Senswave.Automations.Application.Features.DeleteCondition;

public class DeleteConditionHandler(
    ICommandConditionRepository commandConditionRepository,
    IAutomationAccessService automationAccessService)
    : ICommandHandler<DeleteConditionCommand>
{
    public async Task<Result> Handle(DeleteConditionCommand request, CancellationToken cancellationToken)
    {
        var automationCondition =
            await commandConditionRepository.GetAutomationCondition(request.ConditionId, cancellationToken);

        if (automationCondition is null)
            return Result.Failure([DeleteConditionErrors.AutomationConditionNotFound]);

        var isOwner = automationAccessService.IsOwner(automationCondition.Automation.HomesReference, request.UserId);
        if (!isOwner)
            return Result.Failure([DeleteConditionErrors.UserHasNoAccess]);

        // TODO: Uncomment after implementing Put Condition(test issue)
        // User can not delete last condition from automation
        // if (automationCondition.Automation.Conditions.Count == 1)
        //     return Result.Failure([DeleteConditionErrors.DeleteLastCondition]);

        var dbTransactionResult =
            await commandConditionRepository.DeleteAutomationCondition(automationCondition, cancellationToken);

        if (dbTransactionResult.IsFailure)
            return Result.Failure([DeleteConditionErrors.CanNotDeleteCondition]);

        return Result.Success();
    }
}