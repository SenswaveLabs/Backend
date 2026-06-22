namespace Senswave.Infrastructure.Shutdown;


/// <summary>
/// Defines a contract for gracefully shutting down a service or component.
/// </summary>
/// <remarks>Implementations of this interface should ensure that any necessary cleanup or finalization is
/// performed during the shutdown process. This may include releasing resources, completing pending operations, or
/// saving state.</remarks>
public interface IGracefulShutdown
{
    Task StopAsync(CancellationToken cancellationToken = default);
}
