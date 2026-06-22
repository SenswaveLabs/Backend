using Microsoft.EntityFrameworkCore;
using Senswave.Integration.Automations.Remove;
using Senswave.Integration.Shared;

namespace Senswave.Automations.Infrastructure.Consumers;

public class AutomationsRemovalConsumer(
    AutomationsContext automationsContext,
    ILogger<AutomationsRemovalConsumer> logger) : IConsumer<AutomationsRemoveRequest>
{
    public async Task Consume(ConsumeContext<AutomationsRemoveRequest> context)
    {
        try
        {
            var homerReferences = await automationsContext.HomeReferences
                .Where(hr => hr.OwnerId == context.Message.UserId)
                .ToListAsync(context.CancellationToken);

            var automations = await automationsContext.Automations
                .Where(a => a.HomesReference.OwnerId == context.Message.UserId)
                .ToListAsync(context.CancellationToken);

            automationsContext.HomeReferences.RemoveRange(homerReferences);
            automationsContext.Automations.RemoveRange(automations);

            await automationsContext.SaveChangesAsync(context.CancellationToken);

            await context.RespondAsync<AutomationsRemoveResponse>(BaseInternalResponse.Success());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[User: {userId}] Error occurred while removing automations.", context.Message.UserId);
            var response = BaseInternalResponse.Failure(Error.ServerError("AutomationsRemovalServerError", "Failed to remove automations. Please try again or remove automations and conditions manually.")) as AutomationsRemoveResponse;
            await context.RespondAsync(response!);
        }
    }
}
