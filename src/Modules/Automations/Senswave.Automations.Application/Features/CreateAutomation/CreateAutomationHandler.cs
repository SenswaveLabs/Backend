using Senswave.Automations.Domain.Entities;
using Senswave.Automations.Domain.Factory;
using Senswave.Automations.Domain.Options;
using Senswave.Automations.Domain.Repositories;
using Senswave.Automations.Domain.Services;
using Senswave.Integration.Homes.Home;

namespace Senswave.Automations.Application.Features.CreateAutomation;

public class CreateAutomationHandler(
    IOptions<AutomationOptions> options,
    IAutomationAccessService automationAccessService,
    ConditionFactory conditionFactory,
    IQueryAutomationRepository queryRepository,
    ICommandAutomationRepository commandRepository,
    IRequestClient<HomeRequest> homeRequestClient,
    ILogger<CreateAutomationHandler> logger) : ICommandHandler<CreateAutomationCommand, Automation>
{
    public async Task<Result<Automation>> Handle(CreateAutomationCommand request, CancellationToken cancellationToken)
    {
        var homeOwnerResult = await GetHomeOwner(request.HomeId);
        if (homeOwnerResult.IsFailure)
        {
            logger.LogError("[Home: {homeId}] Failed to get home owner.", request.HomeId);
            return Result<Automation>.Failure(CreateAutomationErrors.FailedToGetHomeOwner);
        }

        if (!await CheckPrivilege(request, homeOwnerResult.Data, cancellationToken))
            return Result<Automation>.Failure(CreateAutomationErrors.AccessDenied);

        var conditions = await conditionFactory.Create(request.Conditions, cancellationToken);
        if (conditions.Any(c => c.IsFailure))
            return Result<Automation>.Failure(CreateAutomationErrors.ConditionConfigurationValidationError);

        var automationWithTheSameName = await queryRepository
            .GetAutomationByName(request.HomeId, request.Name, cancellationToken);

        if (automationWithTheSameName != null)
            return Result<Automation>.Failure(CreateAutomationErrors.NameAlreadyExists);

        // TODO: Redis Lock
        // TODO: Verify nodes using graphs

        var automationsCount = await queryRepository.CountAutomationsByHome(request.HomeId, cancellationToken);

        if (options.Value.Limits.AutomationsPerHome <= automationsCount)
        {
            logger.LogWarning("[Home: {homeId}] Automations limit reached.", request.HomeId);
            return Result<Automation>.Failure(CreateAutomationErrors.AutomationsLimitReached);
        }

        var automation = new Automation
        {
            HomesReference = await GetHomeReference(request.HomeId, homeOwnerResult.Data, cancellationToken),

            Name = request.Name,
            Icon = request.Icon,
            IsEnabled = request.IsEnabled,

            Conditions = request.Conditions,
            ConditionsConnector = request.ConditionConnector,
            Results = request.Results,

            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        var result = await commandRepository.CreateAutomation(automation, cancellationToken);

        if (result.IsFailure)
            return Result<Automation>.Failure(CreateAutomationErrors.FailedToCreate);

        logger.LogInformation("[Automation: {automationId}] Automation created.", automation.Id);

        return Result<Automation>.Success(automation);
    }

    private async Task<bool> CheckPrivilege(CreateAutomationCommand request, Guid ownerId, CancellationToken cancellationToken)
    {
        var homeReference = new HomeReference { HomeId = request.HomeId, OwnerId = ownerId };
        var homeAccess = await automationAccessService.CanManageHome(homeReference, request.UserId, cancellationToken);

        if (homeAccess.IsFailure)
            return false;

        var conditionOperationIds = request.Conditions
            .Select(x => x.OperationId)
            .ToList();

        var resultOperationIds = request.Results
            .Select(x => x.OperationId)
            .ToList();

        var allOperations = conditionOperationIds
            .Union(resultOperationIds)
            .ToList();

        var operationAccess = await automationAccessService.CanActDevices(allOperations, request.UserId, cancellationToken);

        if (operationAccess.IsFailure)
            return false;

        return true;
    }

    private async Task<HomeReference> GetHomeReference(Guid homeId, Guid ownerId, CancellationToken cancellationToken)
    {
        var homeReference = await commandRepository.GetHomeReference(homeId, ownerId, cancellationToken);
        if (homeReference is null)
        {
            homeReference = new HomeReference { HomeId = homeId, OwnerId = ownerId };

            var isSavedToDb = await commandRepository.CreateHomeReference(homeReference, cancellationToken);
            if (isSavedToDb.IsSuccess)
                logger.LogDebug("[HomeReference] Created new home reference for Home: {homeId}, Owner: {ownerId}", homeId, ownerId);
            else
                logger.LogError("[HomeReference] Failed to create home reference for Home: {homeId}, Owner: {ownerId}", homeId, ownerId);
        }
        return homeReference;
    }

    private async Task<Result<Guid>> GetHomeOwner(Guid homeId)
    {
        // TODO: Consider creating special service for this
        var response = await homeRequestClient
            .GetResponse<HomeResponse>(new HomeRequest { HomeId = homeId });
        if (response.Message.IsFailure)
        {
            logger.LogError("[Home: {homeId}] Failed to get home owner from Home module ", homeId);
            return Result<Guid>.Failure(response.Message.Error);
        }

        return Result<Guid>.Success(response.Message.OwnerId);
    }
}