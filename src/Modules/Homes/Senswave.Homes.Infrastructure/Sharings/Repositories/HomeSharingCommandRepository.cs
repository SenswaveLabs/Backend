using Microsoft.EntityFrameworkCore;
using Senswave.Homes.Domain.Sharings.Entities;
using Senswave.Homes.Domain.Sharings.Repositories;

namespace Senswave.Homes.Infrastructure.Sharings.Repositories;

internal sealed class HomeSharingCommandRepository(
    ILogger<HomeSharingCommandRepository> logger,
    HomesContext context) : IHomeSharingCommandRepository
{
    public async Task<Result> CreateHomeSharing(HomeSharing homeSharing, CancellationToken cancellationToken)
    {
        using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var invitation = await context.HomeInvitations
                .Where(x => x.HomeId == homeSharing.HomeId)
                .Where(x => x.FriendId == homeSharing.UserId)
                .Where(x => x.Type == homeSharing.SharingType)
                .FirstOrDefaultAsync(cancellationToken);

            if (invitation is not null)
            {
                context.Remove(invitation);
            }

            await context.HomeSharings.AddAsync(homeSharing, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create HomeSharing");
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.ServerError("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> CreateHomeSharingInvitation(HomeSharingInvitation invitation, CancellationToken cancellationToken)
    {
        using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await context.HomeInvitations.AddAsync(invitation, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create HomeSharingInvitation");
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.ServerError("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> DeleteSharing(HomeSharing homeSharing, CancellationToken cancellationToken)
    {
        using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            context.HomeSharings.Remove(homeSharing);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete HomeSharing");
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.ServerError("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> DeleteSharingInvitation(HomeSharingInvitation invitationAlreadyExists, CancellationToken cancellationToken)
    {
        using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            context.HomeInvitations.Remove(invitationAlreadyExists);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete HomeSharingInvitation");
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.ServerError("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public Task<HomeSharing?> GetHomeSharing(Guid homeSharingId, CancellationToken cancellationToken) => context.HomeSharings
        .Where(x => x.Id == homeSharingId)
        .FirstOrDefaultAsync(cancellationToken);

    public Task<HomeSharing?> GetHomeSharingForUser(Guid homeId, Guid userId, CancellationToken cancellationToken) => context.HomeSharings
        .Where(x => x.HomeId == homeId)
        .Where(x => x.UserId == userId)
        .FirstOrDefaultAsync(cancellationToken);

    public Task<List<HomeSharingInvitation>> GetInvitationsByUser(Guid currentRequestUserId, CancellationToken cancellationToken) => context.HomeInvitations
        .Where(x => x.FriendId == currentRequestUserId)
        .ToListAsync(cancellationToken);

}
