using Microsoft.EntityFrameworkCore;
using Senswave.Homes.Domain.Rooms.Entities;
using Senswave.Homes.Domain.Rooms.Repositories;

namespace Senswave.Homes.Infrastructure.Rooms.Repositories;

internal sealed class RoomCommandRepository(
    HomesContext context,
    ILogger<RoomCommandRepository> logger) : IRoomCommandRepository
{
    public async Task<Result> CreateRoom(Room room, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await context.AddAsync(room, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create room");
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.ServerError("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> UpdateRoom(Room room, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            context.Update(room);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Room: {roomId}] Failed to update room.", room.Id);
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.ServerError("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> DeleteRoom(Room room, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            context.Remove(room);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Room: {roomId}] Failed to remove room.", room.Id);
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.ServerError("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public Task<Room?> GetRoom(Guid roomId, CancellationToken cancellationToken) => context.Rooms
        .Include(x => x.Home)
        .Where(x => x.Id == roomId)
        .FirstOrDefaultAsync(cancellationToken);
}
