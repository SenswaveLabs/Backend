using Senswave.Abstractions.Resulting;
using Senswave.Automations.Api.Features.CreateAutomation;
using Senswave.Presentation.Seed.Automations.Clients;
using Senswave.Presentation.Seed.Automations.Interfaces;
using System.Text.Json.Nodes;

namespace Senswave.Presentation.Seed.Automations.Services;

public class AutomationSeedingService(
    IAutomationClient automationClient,
    ILogger<AutomationSeedingService> logger) : IAutomationSeedingService
{

    private readonly string _booleanCondition = "BooleanCondition";
    private readonly string _textCondition = "TextCondition";
    private readonly string _andConditionConnector = "And";
    private readonly string _orConditionConnector = "Or";

    public async Task<Result> SeedMovementDetection(
        string accessToken,
        Guid homeId,
        Guid movementId,
        Guid controllerStateId,
        Guid controllerOptionId,
        Guid controllerBrightnessId)
    {
        var movementCondition = CreateCondition(movementId, _booleanCondition, new JsonObject { ["isOn"] = true });
        var isOnCondition = CreateCondition(controllerStateId, _booleanCondition, new JsonObject { ["isOn"] = true });

        var changeOptionResult = CreateResult(controllerOptionId, JsonValue.Create("alarm"));
        var changeBrightnessResult = CreateResult(controllerBrightnessId, JsonValue.Create("100"));

        var automation = new CreateAutomationRequest
        {
            HomeId = homeId,
            Icon = "bulb",
            Name = "Movement Detection",
            ConditionConnector = _andConditionConnector,
            Conditions = [movementCondition, isOnCondition],
            Results = [changeOptionResult, changeBrightnessResult],
        };
        try
        {
            await automationClient.CreateAutomation(accessToken, automation);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Home: {homeId}] Error seeding Movement Detection Automation.", homeId);
            return Result.Failure();
        }

        return Result.Success();
    }

    public async Task<Result> SeedDiscoMode(
        string accessToken,
        Guid homeId,
        Guid controllerOptionId,
        Guid controllerBrightnessId)
    {
        var discoOptionCondition = CreateCondition(controllerOptionId, _textCondition,
            new JsonObject { ["requiredValue"] = "Disco" });
        var partyOptionCondition = CreateCondition(controllerOptionId, _textCondition,
            new JsonObject { ["requiredValue"] = "Party" });
        var policeOptionCondition = CreateCondition(controllerOptionId, _textCondition,
            new JsonObject { ["requiredValue"] = "Police" });

        var changeBrightnessResult = CreateResult(controllerBrightnessId, JsonValue.Create("100"));
        var automation = new CreateAutomationRequest
        {
            HomeId = homeId,
            Icon = "happy-outline",
            Name = "Disco Mode",
            ConditionConnector = _orConditionConnector,
            Conditions = [discoOptionCondition, partyOptionCondition, policeOptionCondition],
            Results = [changeBrightnessResult]
        };

        try
        {
            var automationCreatedResponse = await automationClient.CreateAutomation(accessToken, automation);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Home: {homeId}] Error seeding Disco Mode Automation.", homeId);
            return Result.Failure();
        }

        return Result.Success();
    }


    private ConditionDto CreateCondition(Guid operationId, string conditionType, JsonObject configuration) => new()
    {
        OperationId = operationId,
        ConditionType = conditionType,
        ConditionConfiguration = configuration
    };

    private ResultDto CreateResult(Guid operationId, JsonValue valueToSend) => new()
    {
        OperationId = operationId,
        ValueToSend = valueToSend
    };
}