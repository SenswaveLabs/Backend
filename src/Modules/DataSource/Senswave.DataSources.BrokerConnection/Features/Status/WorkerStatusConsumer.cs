using MassTransit;
using Microsoft.Extensions.Logging;
using Senswave.Integration.DataSource.BrokerConnection.WorkerStatus;

namespace Senswave.DataSources.BrokerConnection.Features.Status;

public class WorkerStatusConsumer(ILogger<WorkerStatusConsumer> logger) : IConsumer<WorkerStatusRequest>
{
    public async Task Consume(ConsumeContext<WorkerStatusRequest> context)
    {
        logger.LogDebug("Worker status request received. Worker is ok.");
        await context.RespondAsync(new WorkerStatusResponse());
    }
}
