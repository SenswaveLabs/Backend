using Senswave.Automations.Domain.Repositories;
using Senswave.Automations.Domain.Services;
using Senswave.Integration.Devices.OperationNameById;

namespace Senswave.Automations.Application.Features.DisplayAutomations;

public class DisplayAutomationsHandler(
    IAutomationAccessService automationAccessService,
    IRequestClient<OperationNameByIdRequest> operationNameClient,
    IQueryAutomationRepository queryAutomationRepository,
    ILogger<DisplayAutomationsHandler> logger)
    : IQueryHandler<DisplayAutomationsQuery, IList<Models.AutomationModel>>
{
    public async Task<Result<IList<Models.AutomationModel>>> Handle(DisplayAutomationsQuery request, CancellationToken cancellationToken)
    {
        // Check if user has got access to get this automations
        var homeReference = await queryAutomationRepository.GetHomeReference(request.HomeId, cancellationToken);

        if (homeReference is null)
        {
            logger.LogWarning("[Home: {homeId}] Home reference not found.", request.HomeId);
            return Result<IList<Models.AutomationModel>>.Failure([DisplayAutomationsErrors.HomeReferenceNotFound]);
        }

        var homeAccess = await automationAccessService.CanDisplayHome(homeReference, request.UserId, cancellationToken);

        if (homeAccess.IsFailure)
            return Result<IList<Models.AutomationModel>>.Failure([DisplayAutomationsErrors.AutomationsNotFound]);

        var automations = await queryAutomationRepository.GetAutomationByHomeId(request.HomeId, cancellationToken);

        var allOperations = new HashSet<Guid>();

        foreach (var automation in automations)
        {
            foreach (var condition in automation.Conditions)
            {
                allOperations.Add(condition.OperationId);
            }

            foreach (var result in automation.Results)
            {
                allOperations.Add(result.OperationId);
            }
        }

        var guidToNameResponse = await operationNameClient.GetResponse<OperationNameByIdsResponse>(
            new OperationNameByIdRequest { OperationIds = allOperations }, cancellationToken);

        var automationsDto = automations
            .Select(x => Models.AutomationModel.ToDto(x, guidToNameResponse.Message.IdToName))
            .ToList();

        return Result<IList<Models.AutomationModel>>.Success(automationsDto);
    }
}