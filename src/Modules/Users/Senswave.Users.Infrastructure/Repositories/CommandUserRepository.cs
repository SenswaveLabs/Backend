using Microsoft.EntityFrameworkCore;
using Senswave.Users.Domain.Entity;
using Senswave.Users.Domain.Repositories;

namespace Senswave.Users.Infrastructure.Repositories;

internal sealed class CommandUserRepository(
    UsersContext context,
    ILogger<CommandUserRepository> logger) : ICommandUserRepository
{
    public Task<User?> GetUser(Guid userId, CancellationToken cancellation) => context.Users
        .Where(u => u.Id == userId)
        .FirstOrDefaultAsync(cancellation);

    public Task<User?> GetUserWithConsents(Guid userId, CancellationToken cancellation) => context.Users
        .Include(x => x.UserConsents)
        .Where(u => u.Id == userId)
        .FirstOrDefaultAsync(cancellation);

    public async Task<Result> CreateUserConsents(UserConsents consents, CancellationToken cancellationToken)
    {
        var tranasaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            context.UserConsents.Add(consents);
            await context.SaveChangesAsync(cancellationToken);
            await tranasaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update user consents");
            await tranasaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.ServerError("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> UpdateUser(User user, CancellationToken cancellationToken)
    {
        var tranasaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            context.Users.Update(user);
            await context.SaveChangesAsync(cancellationToken);
            await tranasaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update user");
            await tranasaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.ServerError("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> RemoveUser(User user, RemovedUser removedUser, CancellationToken cancellationToken)
    {
        try
        {
            context.Users.Remove(user);
            context.RemovedUsers.Add(removedUser);
            await context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(Error.ServerError("TransactionFailed", ex.Message));
        }
    }
}
