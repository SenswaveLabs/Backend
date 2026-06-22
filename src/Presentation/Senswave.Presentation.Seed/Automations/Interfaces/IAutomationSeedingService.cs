using Senswave.Abstractions.Resulting;

namespace Senswave.Presentation.Seed.Automations.Interfaces;

public interface IAutomationSeedingService
{
    Task<Result> SeedMovementDetection(
        string accessToken,
        Guid homeId,
        Guid movementId,
        Guid controllerStateId,
        Guid controllerOptionId,
        Guid controllerBrightnessId);

    Task<Result> SeedDiscoMode(
        string accessToken,
        Guid homeId,
        Guid controllerOptionId,
        Guid controllerBrightnessId);

}