using Microsoft.EntityFrameworkCore;
using Senswave.DataSources.Domain.Brokers.Sessions.Entities;
using Senswave.DataSources.Domain.Brokers.Sessions.Repositories;

namespace Senswave.DataSources.Infrastructure.Brokers.Sessions.Repositories;

internal class SessionCommandRepository(
    DataSourcesContext context,
    ILogger<SessionCommandRepository> logger) : ISessionCommandRepository
{
    public async Task<Result> CreateSessionLog(Guid sessionId, Log log, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var session = await context.Sessions
                .FirstAsync(s => s.Id == sessionId, cancellationToken);

            log.Session = session;
            await context.AddAsync(log, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Session: {sessionId}] Failed to create log for session.", sessionId);
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.Failure("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> CreateSessionLogForCurrentSession(Guid brokerId, Log log, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var session = await context.Sessions
                .Where(x => x.Broker.Id == brokerId)
                .OrderBy(x => x.CreatedAtUtc)
                .LastAsync(cancellationToken);

            log.Session = session;
            await context.AddAsync(log, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Broker: {brokerId}] Failed to create log for broker session.", brokerId);
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.Failure("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> FinishSessions(Guid brokerId, CancellationToken cancellationToken)
    {
        var unifinishedSessions = await context.Sessions
            .Where(x => x.BrokerId == brokerId)
            .Where(x => x.Finished == false)
            .ToListAsync(cancellationToken);

        if (unifinishedSessions.Count == 0)
            return Result.Success();

        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var session in unifinishedSessions)
            {
                session.Finished = true;
            }

            context.UpdateRange(unifinishedSessions);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Broker: {brokerId}] Failed to end sessions.", brokerId);
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.Failure("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> FinishSession(Session session, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            session.Finished = true;
            context.Update(session);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Session: {sessionId}] Failed to end session.", session.Id);
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.Failure("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> FinishSession(Guid sessionId, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var session = await context.Sessions
                .FirstAsync(x => x.Id == sessionId, cancellationToken);

            session.Finished = true;
            context.Update(session);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Session: {sessionId}] Failed to end session.", sessionId);
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.Failure("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> CreateSession(Session session, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await context.AddAsync(session, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Session: {sessionId}] Failed to create session", session.Id);
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.Failure("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }
}
