using Senswave.Automations.Domain.Enums;
using Senswave.Automations.Domain.Factory;
using Senswave.Automations.Domain.Repositories;
using Senswave.Automations.Domain.Services;

namespace Senswave.Automations.Application.Features.UpdateAutomation;

public class UpdateAutomationHandler(
    ICommandAutomationRepository commandAutomationRepository,
    IAutomationAccessService accessService,
    ConditionFactory conditionFactory,
    ILogger<UpdateAutomationHandler> logger) : ICommandHandler<UpdateAutomationCommand>
{
    public async Task<Result> Handle(UpdateAutomationCommand request, CancellationToken cancellationToken)
    {
        var automation = await commandAutomationRepository
            .GetAutomation(request.AutomationId, cancellationToken);

        if (automation == null)
            return Result.Failure(UpdateAutomationErrors.AutomationNotFound);

        var homeAccess = await accessService
            .CanManageHome(automation.HomesReference, request.UserId, cancellationToken);

        if (homeAccess.IsFailure)
            return Result.Failure(UpdateAutomationErrors.AccessDenied);

        var oldConditionOperation = automation.Conditions.Select(x => x.OperationId).ToList();
        var conditionOperations = request.Conditions.Select(x => x.OperationId).ToList();
        var oldResultOperations = automation.Results.Select(x => x.OperationId).ToList();
        var resultOperations = request.Results.Select(x => x.OperationId).ToList();
        var allOperations = conditionOperations
            .Concat(resultOperations)
            .Concat(oldConditionOperation)
            .Concat(oldResultOperations)
            .ToList();

        if (allOperations.Count != 0)
        {
            var deviceAccess = await accessService.CanActDevices(allOperations, request.UserId,
                cancellationToken);

            if (deviceAccess.IsFailure)
                return Result.Failure(UpdateAutomationErrors.AccessDenied);
        }

        if (request.Conditions.Count != 0)
        {
            var conditions = await conditionFactory.Create(request.Conditions, cancellationToken);
            if (conditions.Any(x => x.IsFailure))
                return Result.Failure(UpdateAutomationErrors.ConditionConfigurationValidationError);

            automation.Conditions = request.Conditions;
        }

        if (request.Results.Count != 0)
            automation.Results = request.Results;

        if (request.Name != string.Empty)
            automation.Name = request.Name;

        if (request.Icon != string.Empty)
            automation.Icon = request.Icon;

        if (request.ConditionConnector != AutomationConditionConnector.Empty)
            automation.ConditionsConnector = request.ConditionConnector;

        // TODO: Redis Lock
        // TODO: Verify nodes using graphs

        var updateResult = await commandAutomationRepository.UpdateAutomation(automation, cancellationToken);

        if (updateResult.IsFailure)
            return Result.Failure(UpdateAutomationErrors.AutomationUpdateFailed);

        logger.LogInformation("[Automation: {automationId}] Automation updated.", automation.Id);

        return Result.Success();
    }
}