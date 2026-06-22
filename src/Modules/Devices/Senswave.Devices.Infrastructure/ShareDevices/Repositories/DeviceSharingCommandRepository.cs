using Microsoft.EntityFrameworkCore;
using Senswave.Devices.Domain.ShareDevices.Entites;
using Senswave.Devices.Domain.ShareDevices.Enums;
using Senswave.Devices.Domain.ShareDevices.Repositories;

namespace Senswave.Devices.Infrastructure.ShareDevices.Repositories;

public class DeviceSharingCommandRepository(
    DevicesContext context,
    ILogger<DeviceSharingCommandRepository> logger) : IDeviceSharingCommandRepository
{
    public async Task<DeviceSharing?> GetDeviceSharing(Guid deviceSharing, CancellationToken cancellationToken) => await context.DeviceSharings
            .Include(x => x.Device)
            .Where(x => x.Id == deviceSharing)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<Result> DeleteDeviceSharing(DeviceSharing deviceSharing, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            context.DeviceSharings.Remove(deviceSharing);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(ex, "Failed to create device sharing");
            return Result.Failure(Error.Failure("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> CreateOrUpdateDeviceSharing(Guid friendId, Guid deviceId, DeviceSharingType deviceSharing, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var sharing = await context.DeviceSharings
                .Where(x => x.UserId == friendId)
                .Where(x => x.DeviceId == deviceId)
                .FirstOrDefaultAsync(cancellationToken);

            if (sharing != null)
            {
                sharing.SharingType = deviceSharing;
                context.DeviceSharings.Update(sharing);
            }
            else
            {
                var newSharing = new DeviceSharing
                {
                    UserId = friendId,
                    DeviceId = deviceId,
                    SharingType = deviceSharing
                };

                await context.DeviceSharings.AddAsync(newSharing, cancellationToken);
            }

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(ex, "Failed to create device sharing");
            return Result.Failure(Error.Failure("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> DeleteDevicesSharings(Guid userId, Guid homeReferenceId, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var sharings = await context.DeviceSharings
                .Where(x => x.UserId == userId)
                .Where(x => x.Device.HomeReference.HomeId == homeReferenceId)
                .ToListAsync(cancellationToken);

            context.DeviceSharings.RemoveRange(sharings);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete device sharings");
            return Result.Failure(Error.Failure("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }
}