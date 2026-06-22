using Microsoft.EntityFrameworkCore;
using Senswave.Automations.Domain.Entities;
using Senswave.Automations.Domain.Repositories;

namespace Senswave.Automations.Infrastructure.Repositories;

public class CommandResultRepository(
    AutomationsContext context,
    ILogger<CommandResultRepository> logger)
    : ICommandResultRepository
{
    public async Task<AutomationResult?> GetAutomationResult(Guid resultId, CancellationToken cancellationToken) =>
        await context.AutomationResults
            .Where(x => x.Id == resultId)
            .Include(x => x.Automation)
                .ThenInclude(automation => automation.HomesReference)
            .Include(x => x.Automation)
                .ThenInclude(a => a.Results)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<Result> DeleteResult(AutomationResult result, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            context.AutomationResults.Remove(result);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(ex, "[AutomationResult: {resultId}] Failed to delete automation result.", result.Id);
            return Result.Failure(Error.ServerError("TransactionError", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> AddResult(AutomationResult result, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            context.AutomationResults.Add(result);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(ex, "[AutomationResult: {automationId}] Failed to add automationResult.", result.Id);
            return Result.Failure(Error.ServerError("TransactionError", "Unexpected error occurred. Please try again."));
        }
    }
}