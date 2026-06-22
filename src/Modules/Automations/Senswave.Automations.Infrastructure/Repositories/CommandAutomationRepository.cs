using Microsoft.EntityFrameworkCore;
using Senswave.Automations.Domain.Entities;
using Senswave.Automations.Domain.Repositories;

namespace Senswave.Automations.Infrastructure.Repositories;

internal sealed class CommandAutomationRepository(
    AutomationsContext context,
    ILogger<CommandAutomationRepository> logger) : ICommandAutomationRepository
{
    public async Task<Automation?> GetAutomation(Guid automationId, CancellationToken cancellationToken) => await context.Automations
        .Where(x => x.Id == automationId)
        .Include(x => x.Conditions)
        .Include(x => x.Results)
        .Include(x => x.HomesReference)
        .FirstOrDefaultAsync(cancellationToken);

    public async Task<Automation?> GetAutomationToDelete(Guid automationId, CancellationToken cancellationToken)
     => await context.Automations
        .Where(x => x.Id == automationId)
        .Include(x => x.HomesReference)
        .ThenInclude(x => x.Automations)
        .FirstOrDefaultAsync(cancellationToken);

    public async Task<Result> UpdateAutomation(Automation automation, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            context.Automations.Update(automation);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(ex, "[Automation: {automationId}] Failed to update automation.", automation.Id);
            return Result.Failure(Error.ServerError("TransactionError", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> CreateAutomation(Automation automation, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await context.Automations.AddAsync(automation, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(ex, "[Automation: {automationId}] Failed to create automation.", automation.Id);
            return Result.Failure(Error.ServerError("TransactionError", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> DeleteAutomation(Automation automation, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            context.Automations.Remove(automation);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(ex, "[Automation: {automationId}] Failed to delete automation.", automation.Id);
            return Result.Failure(Error.ServerError("TransactionError", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<HomeReference?> GetHomeReference(Guid homeId, Guid userId, CancellationToken cancellationToken)
    {
        return await context.HomeReferences
            .Where(x => x.HomeId == homeId && x.OwnerId == userId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Result> CreateHomeReference(HomeReference homeReference, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await context.HomeReferences.AddAsync(homeReference, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(ex, "[HomeReference] Failed to create home reference for Home: {homeId}, Owner: {ownerId}",
                homeReference.HomeId, homeReference.OwnerId);
            return Result.Failure(Error.ServerError("TransactionError", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> DeleteHomeReference(HomeReference homeReference, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            context.HomeReferences.Remove(homeReference);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(ex, "[HomeReference] Failed to delete home reference for Home: {homeId}, Owner: {ownerId}",
                homeReference.HomeId, homeReference.OwnerId);
            return Result.Failure(Error.ServerError("TransactionError", "Unexpected error occurred. Please try again."));
        }
    }
}
