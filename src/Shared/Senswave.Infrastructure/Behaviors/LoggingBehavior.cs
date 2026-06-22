using MediatR;

namespace Senswave.Infrastructure.Behaviors;

internal sealed class LoggingBehaviour<TRequest, TResponse>(ILogger<IPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2017:Parameter count mismatch", Justification = "")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "")]
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        logger.LogInformation("[CorrelationId: {CorrelationId}] [Path: {Path}] Processing started.");

        var response = await next();

        if (response.IsFailure)
        {
            logger.LogError("[CorrelationId: {CorrelationId}] [Path: {Path}] Procesing failed.");
            return response;
        }

        logger.LogInformation("[CorrelationId: {CorrelationId}] [Path: {Path}] Processing succeded.");
        return response;
    }
}
