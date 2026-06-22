using Microsoft.EntityFrameworkCore;
using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Widgets.Entities;
using Senswave.Devices.Domain.Widgets.Repositories;

namespace Senswave.Devices.Infrastructure.Widgets.Repositories;

public class WidgetCommandRepository(
    DevicesContext context,
    ILogger<WidgetCommandRepository> logger) : IWidgetCommandRepository
{
    public Task<Operation?> GetOperationWithDevice(Guid operationId, CancellationToken cancellationToken) => context.Operations
        .Include(o => o.Device)
        .Where(o => o.Id == operationId)
        .FirstOrDefaultAsync(cancellationToken);

    public Task<Widget?> GetWidget(Guid widgetId, CancellationToken cancellationToken) => context.Widgets
        .Where(x => x.Id == widgetId)
        .Include(x => x.Operation)
        .FirstOrDefaultAsync(cancellationToken);

    public async Task<Result> CreateWidget(Widget widget, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            if (widget.Operation != null)
            {
                context.Operations.Attach(widget.Operation);
            }

            await context.Widgets.AddAsync(widget, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Widget: {widgetId}] Failed to create widget.", widget.Id);
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.ServerError("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> UpdateWidget(Widget widget, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            widget.UpdatedAtUtc = DateTime.UtcNow;
            context.Widgets.Update(widget);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Widget: {widgetId}] Failed to update widget.", widget.Id);
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.ServerError("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public async Task<Result> DeleteWidget(Guid widgetId, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var widget = await context.Widgets
                .Where(x => x.Id == widgetId)
                .FirstOrDefaultAsync(cancellationToken);

            if (widget is null)
                return Result.Success();

            context.Widgets.Remove(widget);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Widget: {widgetId}] Failed to delete widget.", widgetId);
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(Error.ServerError("TransactionFailed", "Unexpected error occurred. Please try again."));
        }
    }

    public Task<bool> WidgetNameUsed(Guid operationId, string name, CancellationToken cancellationToken) => context.Widgets
        .Where(x => x.OperationId == operationId)
        .Where(x => x.Name == name)
        .AnyAsync(cancellationToken);
}