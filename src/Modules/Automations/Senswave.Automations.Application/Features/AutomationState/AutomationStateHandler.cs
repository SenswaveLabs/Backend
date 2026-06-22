using Senswave.Automations.Domain.Repositories;
using Senswave.Automations.Domain.Services;

namespace Senswave.Automations.Application.Features.AutomationState;

public class AutomationStateHandler(
    ICommandAutomationRepository commandAutomationRepository,
    IAutomationAccessService accessService,
    ILogger<AutomationStateHandler> logger) : ICommandHandler<AutomationStateCommand>
{
    public async Task<Result> Handle(AutomationStateCommand request, CancellationToken cancellationToken)
    {
        var automation = await commandAutomationRepository.GetAutomation(request.AutomationId, cancellationToken);

        if (automation == null)
            return Result.Failure([AutomationStateErrors.AutomationNotFound]);

        var canEnable = await accessService.CanManageHome(automation.HomesReference, request.UserId, cancellationToken);

        if (canEnable.IsFailure)
            return Result.Failure([AutomationStateErrors.AccessDenied]);

        automation.IsEnabled = request.IsEnabled;

        var updateResult = await commandAutomationRepository.UpdateAutomation(automation, cancellationToken);

        if (updateResult.IsFailure)
            return Result.Failure(AutomationStateErrors.UpdateFailed, updateResult.Errors);

        logger.LogInformation("[Automation: {automationId}] Automation status changed.", automation.Id);

        return Result.Success();
    }
}