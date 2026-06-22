using Senswave.Automations.Domain.Entities;
using Senswave.Automations.Domain.Factory;
using Senswave.Automations.Domain.Repositories;
using Senswave.Automations.Domain.Services;
using Senswave.Integration.Devices.OperationExists;

namespace Senswave.Automations.Application.Features.PutConditionToAutomation;

public class PutConditionHandler(
    ICommandAutomationRepository commandAutomationRepository,
    IAutomationAccessService automationAccessService,
    IRequestClient<OperationExistsRequest> operationExistsClient,
    ConditionFactory conditionFactory,
    ICommandConditionRepository commandConditionRepository)
    : ICommandHandler<PutConditionCommand>
{
    public async Task<Result> Handle(PutConditionCommand request, CancellationToken cancellationToken)
    {
        var automation = await commandAutomationRepository.GetAutomation(request.AutomationId, cancellationToken);

        if (automation is null)
            return Result.Failure([PutConditionErrors.AutomationNotFound]);

        var isOwner = automationAccessService.IsOwner(automation.HomesReference, request.UserId);

        if (!isOwner)
            return Result.Failure([PutConditionErrors.UserHasNoAccess]);

        var canActOperation =
            await automationAccessService.CanActDevices([request.AutomationCondition!.OperationId], request.UserId, cancellationToken);

        if (!canActOperation)
            return Result.Failure([PutConditionErrors.UserHasNoAccess]);

        var operationExists =
            await operationExistsClient.GetResponse<OperationExistsResponse>(
                new OperationExistsRequest() { OperationId = request.AutomationCondition!.OperationId }, cancellationToken);

        if (operationExists.Message.IsFailure)
            return Result.Failure([PutConditionErrors.OperationNotFound]);

        var condition = await conditionFactory.Create([request.AutomationCondition], cancellationToken);
        if (condition.Any(c => c.IsFailure))
            return Result<Automation>.Failure(PutConditionErrors.ConditionConfigurationValidationError);

        var automationCondition = new AutomationCondition
        {
            Automation = automation,
            OperationId = request.AutomationCondition.OperationId,
            ConditionConfiguration = request.AutomationCondition.ConditionConfiguration,
            ConditionType = request.AutomationCondition.ConditionType,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        var dbTransaction =
            await commandConditionRepository.AddAutomationCondition(automationCondition, cancellationToken);

        if (dbTransaction.IsFailure)
            return Result.Failure([PutConditionErrors.DatabaseError]);

        return Result.Success();
    }
}