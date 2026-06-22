using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Serilog.Context;
using System.Diagnostics;

namespace Senswave.Infrastructure.Web.Diagnostics;

public class DiagnosticsMiddleware(ILogger<DiagnosticsMiddleware> logger) : IMiddleware
{
    private const string CorrelationIdHeaderName = "X-Correlation-Id";

    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();

        var correlationId = GetCorrelationId(context);
        var path = context.Request.Path;

        context.Response.OnStarting(() =>
        {
            stopwatch.Stop();
            logger.LogInformation("[CorrelationId: {CorrelationId}] [Path: {Path}] Executed total of: {ElapsedMilliseconds} ms.", correlationId, path, stopwatch.ElapsedMilliseconds);
            context.Response.Headers["Response-Time"] = stopwatch.ElapsedMilliseconds.ToString();
            return Task.CompletedTask;
        });

        using (LogContext.PushProperty("Path", path))
        {
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                return next.Invoke(context);
            }
        }
    }

    private static string GetCorrelationId(HttpContext context)
    {
        context.Request.Headers.TryGetValue(
            CorrelationIdHeaderName, out StringValues correlationId);

        return correlationId.FirstOrDefault() ?? context.TraceIdentifier;
    }
}

