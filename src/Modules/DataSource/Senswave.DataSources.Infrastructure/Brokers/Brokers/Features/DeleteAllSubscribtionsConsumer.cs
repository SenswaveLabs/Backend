using Senswave.DataSources.Domain.Brokers.Brokers.Repositories;
using Senswave.Integration.DataSource.DeleteAllSubscribtions;
using Senswave.Integration.Shared;

namespace Senswave.DataSources.Infrastructure.Brokers.Brokers.Features;

internal sealed class DeleteAllSubscribtionsConsumer(
    ISubscribtionCommandRepository repository,
    ILogger<DeleteAllSubscribtionsConsumer> logger) : IConsumer<DeleteAllSubscribtionsRequest>
{
    public async Task Consume(ConsumeContext<DeleteAllSubscribtionsRequest> context)
    {
        var removeResult = await repository.DeleteAllSubscribtions(context.Message.DataSourceId, context.CancellationToken);

        var response = BaseInternalResponse.Success();

        if (removeResult.IsFailure)
        {
            var error = removeResult?.Errors[0] ?? Error.Failure("UnexpectedDeleteSubscribtionsError", "An unexpected error occurred while deleting subscriptions.");
            logger.LogError("[DataSource: {dataSourceId}] Failed to delete all subscriptions. Error: {error}", context.Message.DataSourceId, error);
            response = BaseInternalResponse.Failure(error);
        }
        else
        {
            logger.LogInformation("[DataSource: {dataSourceId}] Deleted all subscriptions.", context.Message.DataSourceId);
        }

        await context.RespondAsync<DeleteAllSubscribtionsResponse>(response);
    }
}
