using Senswave.Abstractions.Messaging.Interfaces;
using Senswave.Automations.Domain.Entities;
using Senswave.Automations.Domain.Services;
using Senswave.Integration.Automations.TriggerOperationEvent;

namespace Senswave.Automations.Application.Services;

public class AutomationActionService(
    ILogger<AutomationActionService> logger,
    IAutomationInterpreter automationInterpreter,
    IPublishMessageBus bus) : IAutomationActionService
{
    public async Task ProcessAutomationWithEvent(IList<Automation> automations, CancellationToken cancellationToken)
    {
        var automationsToTrigger = new List<Automation>();

        foreach (var automation in automations)
        {
            logger.LogInformation("[Autaomtion: {automation}] Processing automation.",
                automation.Id);

            if (!automation.IsEnabled)
            {
                logger.LogDebug("[Automation: {automation}] Automation is disabled, skipping.", automation.Id);
                continue;
            }

            if (await automationInterpreter.Interpret(automation, cancellationToken))
                automationsToTrigger.Add(automation);
        }

        var resultsToTrigger = automationsToTrigger
            .SelectMany(a => a.Results)
            .ToList();

        if (resultsToTrigger.Count == 0)
        {
            logger.LogDebug("No automation results to trigger.");
            return;
        }

        var operationsWithValues = resultsToTrigger
            .Select(TriggerOperationWithValue)
            .ToList();

        var eventMessage = new ExternalDeviceActionRequest { OperationsWithValues = operationsWithValues };

        await bus.Publish(eventMessage, cancellationToken);
    }

    private TriggerOperationWithValue TriggerOperationWithValue(AutomationResult result) => new()
    {
        OperationId = result.OperationId,
        Value = result.ValueToSend
    };
}