using FluentValidation;
using MediatR;
using Senswave.Infrastructure.Diagnostics;

namespace Senswave.Infrastructure.Behaviors;

internal sealed class ValidationBehaviour<TRequest, TResponse>(
    IInfrastructureDiagnosticsActivityProvider activityProvider,
    IEnumerable<IValidator<TRequest>> validators,
    ILogger<IPipelineBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2017:Parameter count mismatch", Justification = "")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "")]
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            logger.LogInformation("[CorrelationId: {CorrelationId}] [Path: {Path}] Validation skip.");
            return await next();
        }

        using (var activity = activityProvider.StartActivity("infrastructure.validation"))
        {
            activity?.AddTag("infrastructure.validation.length", validators.Count());

            var errors = validators.Select(validator => validator.Validate(request))
                .SelectMany(validationResult => validationResult.Errors)
                .Where(validationFailure => validationFailure is not null)
                .Select(error => Error.Validation(error.ErrorCode, description: error.ErrorMessage))
                .Distinct()
                .ToArray();

            if (errors.Length != 0)
            {
                logger.LogError("[CorrelationId: {CorrelationId}] [Path: {Path}] Validation failed: {@errors}.", errors);

                if (typeof(TResponse) == typeof(Result) || typeof(TResponse) == typeof(Result))
                    return (Result.Failure(errors) as TResponse)!;

                var methods = typeof(Result<>)!
                    .GetGenericTypeDefinition()!
                    .MakeGenericType(typeof(TResponse)
                    .GenericTypeArguments[0])!
                    .GetMethods();

                var method = methods.FirstOrDefault(
                    m => m.Name == nameof(Result.Failure) &&
                    m.GetParameters().Length == 1 &&
                    m.GetParameters()[0].ParameterType == typeof(Error[]))!;

                var result = method.Invoke(null, [errors])!;

                activity?.AddEvent(new("Validation failed."));

                return (TResponse)result;
            }

            activity?.AddEvent(new("Validated."));
            logger.LogInformation("[CorrelationId: {CorrelationId}] [Path: {Path}] Validation success.");
        }

        return await next();
    }
}
