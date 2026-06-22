using Microsoft.EntityFrameworkCore;
using Senswave.Integration.Homes.Remove;
using Senswave.Integration.Shared;

namespace Senswave.Homes.Infrastructure.Consumers;

internal class HomeRemovalConsumer(
    HomesContext homesContext,
    ILogger<HomeRemovalConsumer> logger) : IConsumer<HomesRemoveRequest>
{
    public async Task Consume(ConsumeContext<HomesRemoveRequest> context)
    {
        try
        {
            var entities = await homesContext.Homes
                .Where(hr => hr.OwnerId == context.Message.UserId)
                .ToListAsync(context.CancellationToken);

            var sharings = await homesContext.HomeSharings
                .Where(hs => hs.UserId == context.Message.UserId)
                .ToListAsync(context.CancellationToken);

            homesContext.Homes.RemoveRange(entities);
            homesContext.HomeSharings.RemoveRange(sharings);
            await homesContext.SaveChangesAsync(context.CancellationToken);
            await context.RespondAsync<HomesRemoveResponse>(BaseInternalResponse.Success());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[User: {userId}] Error occurred while removing homes.", context.Message.UserId);
            var response = BaseInternalResponse.Failure(Error.ServerError("HomesRemovalServerError", "Failed to remove homes or linked entites. Please try again or remove homes, rooms etc. manually.")) as HomesRemoveResponse;
            await context.RespondAsync(response!);
        }
    }
}
