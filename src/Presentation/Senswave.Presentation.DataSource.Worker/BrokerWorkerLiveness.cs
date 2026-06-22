namespace Senswave.Presentation.DataSource.Worker;

public class BrokerWorkerLiveness(ILogger<BrokerWorkerLiveness> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            await Task.Delay(15000, stoppingToken);
        }
    }
}
