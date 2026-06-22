using Senswave.Automations.Domain.Repositories;
using Senswave.Automations.Domain.Services;

namespace Senswave.Automations.Application.Features.DeleteAutomation;

public class DeleteAutomationHandler(
    IAutomationAccessService automationAccessService,
    ICommandAutomationRepository commandAutomationRepository)
    : ICommandHandler<DeleteAutomationCommand>
{
    public async Task<Result> Handle(DeleteAutomationCommand request, CancellationToken cancellationToken)
    {
        // Only owner of the home can delete automation
        var automation = await commandAutomationRepository.GetAutomationToDelete(request.AutomationId, cancellationToken);

        if (automation == null)
            return Result.Failure([DeleteAutomationErrors.AutomationNotFound]);

        var isOwner = automationAccessService.IsOwner(automation.HomesReference, request.UserId);

        if (!isOwner)
            return Result.Failure([DeleteAutomationErrors.UserHasNoAccess]);

        if (automation.HomesReference.Automations.Count == 1)
        {
            var deleteHomeReference = await commandAutomationRepository.DeleteHomeReference(automation.HomesReference, cancellationToken);

            if (deleteHomeReference.IsFailure)
                return Result.Failure([DeleteAutomationErrors.CanNotDeleteHomeReference]);
            return Result.Success();
        }

        var automationDeleted = await commandAutomationRepository.DeleteAutomation(automation, cancellationToken);

        if (automationDeleted.IsFailure)
            return Result.Failure([DeleteAutomationErrors.CanNotDeleteAutomation]);

        return Result.Success();
    }
}