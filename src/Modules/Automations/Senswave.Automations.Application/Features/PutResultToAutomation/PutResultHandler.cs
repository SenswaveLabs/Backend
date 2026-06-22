using Senswave.Automations.Domain.Entities;
using Senswave.Automations.Domain.Repositories;
using Senswave.Automations.Domain.Services;
using Senswave.Integration.Devices.OperationExists;

namespace Senswave.Automations.Application.Features.PutResultToAutomation;

public class PutResultHandler(
    IAutomationAccessService automationAccessService,
    ICommandAutomationRepository commandAutomationRepository,
    ICommandResultRepository commandResultRepository,
    ILogger<PutResultHandler> logger,
    IRequestClient<OperationExistsRequest> operationExistsClient)
    : ICommandHandler<PutResultCommand>
{
    public async Task<Result> Handle(PutResultCommand request, CancellationToken cancellationToken)
    {
        // Only owner of the home can modify automation ( like adding results )
        logger.LogInformation("[PutResultHandler] Handle PutResultCommand");
        var automation = await commandAutomationRepository.GetAutomation(request.AutomationId, cancellationToken);
        if (automation is null)
            return Result.Failure([PutResultErrors.AutomationNotFound]);

        var isOwner = automationAccessService.IsOwner(automation.HomesReference, request.UserId);

        if (!isOwner)
            return Result.Failure([PutResultErrors.UserHasNoAccess]);

        var canActOperation =
            await automationAccessService.CanActDevices([request.Result!.OperationId], request.UserId, cancellationToken);

        if (!canActOperation)
            return Result.Failure([PutResultErrors.UserHasNoAccess]);

        var operationExists =
            await operationExistsClient.GetResponse<OperationExistsResponse>(
                new OperationExistsRequest() { OperationId = request.Result!.OperationId }, cancellationToken);

        if (operationExists.Message.IsFailure)
            return Result.Failure([PutResultErrors.OperationNotFound]);

        var automationResult = new AutomationResult
        {
            Automation = automation,
            OperationId = request.Result.OperationId,
            ValueToSend = request.Result.ValueToSend,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };
        var response = await commandResultRepository.AddResult(automationResult, cancellationToken);
        if (response.IsFailure)
            return Result.Failure([PutResultErrors.DatabaseError]);

        return Result.Success();
    }
}