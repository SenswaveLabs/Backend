using Senswave.Automations.Domain.Repositories;
using Senswave.Automations.Domain.Services;
using Senswave.Integration.DataTransfer.MessageOperationProcessed;

namespace Senswave.Automations.Infrastructure.Consumers;

public class AutomationTriggerConsumer(
    ILogger<AutomationTriggerConsumer> logger,
    IQueryAutomationRepository automationRepository,
    IAutomationActionService automationActionService
    ) : IConsumer<ProcessedOperationTriggerAutomationEvent>
{
    public async Task Consume(ConsumeContext<ProcessedOperationTriggerAutomationEvent> context)
    {
        var operationId = context.Message.OperationId;
        var homeId = context.Message.HomeId;

        logger.LogInformation("[Operation: {operation}] [Home: {home}] Starting processing of automations for home.",
            operationId, homeId);

        var automationsToCheck = await automationRepository
            .GetAutomationsByHomeIdAndCondition(homeId, operationId, context.CancellationToken);

        await automationActionService.ProcessAutomationWithEvent(automationsToCheck, context.CancellationToken);
    }
}