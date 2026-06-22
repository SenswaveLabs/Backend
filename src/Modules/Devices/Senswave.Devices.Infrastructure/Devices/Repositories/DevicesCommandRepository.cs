using Microsoft.EntityFrameworkCore;
using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Devices.Repositories;

namespace Senswave.Devices.Infrastructure.Devices.Repositories;

public class DevicesCommandRepository(
    DevicesContext context,
    ILogger<DevicesCommandRepository> logger) : IDeviceCommandRepository
{
    public async Task<Result> CreateDevice(Guid homeId, Guid homeOwnerId, Device device, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var homeReference = await context.HomeReferences
                .Where(x => x.HomeId  == homeId)
                .Where(x => x.OwnerId == homeOwnerId)
                .FirstOrDefaultAsync(cancellationToken);

            if (homeReference is not null)
            {
                device.HomeReferenceId = homeReference.Id;
                device.HomeReference = homeReference;
            }
            else
            {
                var homeRefEntity = new HomeReference()
                {
                    HomeId = homeId,
                    OwnerId = homeOwnerId
                };

                await context.HomeReferences.AddAsync(homeRefEntity, cancellationToken);

                device.HomeReferenceId = homeRefEntity.Id;
                device.HomeReference = homeRefEntity;
            }


            await context.Devices.AddAsync(device, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while creating device.");
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.ServerError("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> UpdateDevice(Device device, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            context.Update(device);
            context.Update(device.Tile);
            context.Update(device.Presence!);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while updating device.");
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.ServerError("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> DeleteDevice(Device device, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            context.Devices.Remove(device);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while deleting device.");
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.ServerError("TransactionFailed", "Unexpected error occurred. Please try again."));

        }
    }

    public Task<Device?> GetDeviceWithTile(Guid deviceId, CancellationToken cancellationToken) => context.Devices
        .Include(x => x.Operations)
        .Include(x => x.HomeReference)
        .Include(x => x.Tile)
            .ThenInclude(x => x!.SwitchOperation)
        .Include(x => x.Tile)
            .ThenInclude(x => x.DisplayableOperation)
        .Include(x => x.Presence)
            .ThenInclude(x => x!.Operation)
        .FirstOrDefaultAsync(x => x.Id == deviceId, cancellationToken);

    public Task<Device?> GetDeviceForDeletion(Guid deviceId, CancellationToken cancellationToken) => context.Devices
        .Where(x => x.Id == deviceId)
        .Include(x => x.Operations)
        .Include(x => x.DataReferences)
        .Include(x => x.Dashboards)
        .Include(x => x.Tile)
            .ThenInclude(x => x!.SwitchOperation)
        .Include(x => x.Tile)
            .ThenInclude(x => x.DisplayableOperation)
        .FirstOrDefaultAsync(cancellationToken);

    public async Task<Result> UpdateDeviceWithAddingPresence(Device device, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            context.Update(device);
            context.Update(device.Tile);
            context.Add(device.Presence!);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while updating device.");
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.ServerError("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }
}