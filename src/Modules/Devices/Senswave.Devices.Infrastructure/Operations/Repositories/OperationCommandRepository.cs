using Microsoft.EntityFrameworkCore;
using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Operations.Repositories;
using Senswave.Devices.Domain.Operations.ValueObjects;

namespace Senswave.Devices.Infrastructure.Operations.Repositories;

internal class OperationCommandRepository(
    DevicesContext context,
    ILogger<OperationCommandRepository> logger) : IOperationCommandRepository
{
    public async Task<bool> UpdateOperationsWithValue(List<Operation> operations, CancellationToken cancellationToken)
    {
        using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            context.Operations.UpdateRange(operations);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(ex, "Failed to commit changes for operation list update.");
            return false;
        }
    }

    public async Task<bool> UpdateOperationWithValue(Operation operation, OperationValue operationValue, CancellationToken cancellationToken)
    {
        using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            operation.Values.Add(operationValue);

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(ex, "[Operation: {operationId}] Failed to commit changes.", operation.Id);
            return false;
        }
    }

    public async Task<Result> CreateOperation(Guid deviceId, Operation operation, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var dataReference = await context.DataReferences
                .Where(x => x.DataSourceDataReferenceId == operation.DataReferenceId)
                .FirstOrDefaultAsync(cancellationToken);

            if (dataReference is null)
            {
                dataReference = new DataSourceDataReference
                {
                    DeviceId = deviceId,
                    DataSourceDataReferenceId = operation.DataReferenceId!.Value,
                };

                await context.DataReferences.AddAsync(dataReference, cancellationToken);
            }
            else
            {
                if (dataReference.DeviceId != deviceId)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Result.Failure(Error.Conflict("DataSourceReferenceAlreadyInUse", "The topic is already in use by another device operation. Topics cannot be shared."));
                }
            }

            operation.DataReference = dataReference;
            await context.Operations.AddAsync(operation, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(ex, "[Device: {deviceId}] Failed to commit changes for new operation.", deviceId);
            return Result.Failure(Error.ServerError("TransactionError", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> DeleteOperation(Guid operationId, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var operation = await context.Operations
                .Include(x => x.DataReference)
                .FirstOrDefaultAsync(x => x.Id == operationId, cancellationToken);

            if (operation is null)
                return Result.Success();

            var isUsed = await context.Operations
                .Where(x => x.DataReferenceId == operation.DataReferenceId)
                .CountAsync(cancellationToken);

            if (isUsed > 1)
            {
                context.Operations.Remove(operation);
            }
            else
            {
                context.Operations.Remove(operation);

                if (operation.DataReference is not null)
                    context.DataReferences.Remove(operation.DataReference!);
            }

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(ex, "[Operation: {operationId}] Failed to commit changes.", operationId);
            return Result.Failure(Error.ServerError("TransactionError", "Unexpected error occurred. Please try again."));
        }
    }

    public Task<Device?> GetDevice(Guid deviceId, CancellationToken cancellationToken) => context.Devices
        .Where(x => x.Id == deviceId)
        .Include(x => x.HomeReference)
        .FirstOrDefaultAsync(cancellationToken);

    public Task<DataSourceDataReference?> GetDataSourceDataReference(Guid subscriptionId, CancellationToken cancellationToken) => context.DataReferences
        .Where(x => x.DataSourceDataReferenceId == subscriptionId)
        .FirstOrDefaultAsync(cancellationToken);

    public Task<List<Operation>> GetOperationsByReference(Guid dataSourceReferenceId, CancellationToken cancellationToken) => context.Operations
        .Where(o => o.DataReferenceId != null && o.DataReference!.DataSourceDataReferenceId == dataSourceReferenceId)
        .Include(o => o.Device)
        .ThenInclude(d => d!.HomeReference)
        .Include(o => o.Values)
        .ToListAsync(cancellationToken);

    public Task<Operation?> GetOperation(Guid operationId, CancellationToken cancellationToken) => context.Operations
        .Include(x => x.DataReference)
        .Include(o => o.Device)
        .ThenInclude(d => d!.HomeReference)
        .Include(x => x.Values)
        .FirstOrDefaultAsync(x => x.Id == operationId, cancellationToken);
}