using Senswave.Automations.Domain.Repositories;
using Senswave.Automations.Domain.Services;
using Senswave.Integration.Devices.OperationNameById;

namespace Senswave.Automations.Application.Features.GetAutomation;

public class GetAutomationHandler(
    IQueryAutomationRepository queryAutomationRepository,
    IAutomationAccessService accessService,
    IRequestClient<OperationNameByIdRequest> operationNameClient)
    : IQueryHandler<GetAutomationQuery, Models.AutomationModel>
{
    public async Task<Result<Models.AutomationModel>> Handle(GetAutomationQuery request, CancellationToken cancellationToken)
    {
        var automation = await queryAutomationRepository.GetAutomation(request.AutomationId, cancellationToken);

        if (automation is null)
            return Result<Models.AutomationModel>.Failure([GetAutomationErrors.AutomationNotFound]);

        var canDisplay = await accessService.CanDisplayHome(automation.HomesReference, request.UserId, cancellationToken);

        if (canDisplay.IsFailure)
            return Result<Models.AutomationModel>.Failure([GetAutomationErrors.AccessDenied]);

        var allOperations = new HashSet<Guid>(
            automation.Conditions.Select(c => c.OperationId)
                .Concat(automation.Results.Select(r => r.OperationId)));

        var guidToNameResponse = await operationNameClient.GetResponse<OperationNameByIdsResponse>(
            new OperationNameByIdRequest { OperationIds = allOperations }, cancellationToken);

        var automationDto = Models.AutomationModel.ToDto(automation, guidToNameResponse.Message.IdToName);

        return Result<Models.AutomationModel>.Success(automationDto);
    }
}