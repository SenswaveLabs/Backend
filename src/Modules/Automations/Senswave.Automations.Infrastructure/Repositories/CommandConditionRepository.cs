using Microsoft.EntityFrameworkCore;
using Senswave.Automations.Domain.Entities;
using Senswave.Automations.Domain.Repositories;

namespace Senswave.Automations.Infrastructure.Repositories;

public class CommandConditionRepository(
    AutomationsContext context,
    ILogger<CommandConditionRepository> logger)
    : ICommandConditionRepository
{
    public async Task<AutomationCondition?> GetAutomationCondition(Guid conditionId, CancellationToken cancellationToken)
        => await context.AutomationConditions
            .Where(condition => condition.Id == conditionId)
            .Include(condition => condition.Automation)
                .ThenInclude(automation => automation.HomesReference)
            .Include(condition => condition.Automation)
                .ThenInclude(automation => automation.Conditions)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<Result> DeleteAutomationCondition(AutomationCondition automationCondition, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            context.AutomationConditions.Remove(automationCondition);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(ex, "[AutomationCondition: {conditionId}] Failed to delete automation condition.", automationCondition.Id);
            return Result.Failure(Error.ServerError("TransactionError", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> AddAutomationCondition(AutomationCondition automationCondition, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            context.AutomationConditions.Add(automationCondition);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(ex, "[AutomationCondition: {automationId}] Failed to add automation condition.", automationCondition.Id);
            return Result.Failure(Error.ServerError("TransactionError", "Unexpected error occurred. Please try again."));
        }
    }
}