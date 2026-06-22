using MassTransit;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Senswave.Integration.DataSource.BrokerConnection.WorkerStatus;

namespace Senswave.DataSources.BrokerConnection.Features.Status;

public class WorkerStatusHealthCheck(IRequestClient<WorkerStatusRequest> requestClient,
    ILogger<WorkerStatusHealthCheck> logger) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var status = await requestClient.GetResponse<WorkerStatusResponse>(new WorkerStatusRequest(), cancellationToken);

            if (status.Message.IsSuccess)
            {
                logger.LogDebug("Data source worker health check succeeded.");
                return HealthCheckResult.Healthy("Data source worker is healthy");
            }

            logger.LogDebug("Data source worker health check reported unhealthy status.");
            return HealthCheckResult.Unhealthy("Data source worker is unhealthy.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Data source worker health check failed with an exception.");
            return HealthCheckResult.Unhealthy("Data source worker health check failed.", ex);
        }
    }
}
