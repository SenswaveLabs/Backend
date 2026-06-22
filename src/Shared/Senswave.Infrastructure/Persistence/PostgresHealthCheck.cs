using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Senswave.Infrastructure.Persistence;

public class PostgresHealthCheck(
    IOptions<PostgreSqlOptions> options,
    ILogger<PostgresHealthCheck> logger) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var connectionString = options.Value.ConnectionString;
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var cmd = new NpgsqlCommand("SELECT 1", connection);
            await cmd.ExecuteScalarAsync(cancellationToken);

            logger.LogDebug("PostgreSQL is healthy");

            return HealthCheckResult.Healthy("PostgreSQL is healthy.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "PostgreSQL health check failed.");
            return HealthCheckResult.Unhealthy("PostgreSQL is unhealthy.", ex);
        }
    }
}
