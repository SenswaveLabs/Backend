
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Senswave.Infrastructure.Web.Diagnostics.Health;

public class HealthCheckMiddleware(IOptionsSnapshot<HealthOptions> options) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (IsHealthRequest(context))
        {
            await MeasureHealth(context);
            return;
        }

        await next(context);
    }

    private bool IsHealthRequest(HttpContext context)
    {
        if (context.Request.Path != options.Value.Path)
            return false;

        if (context.Connection.LocalPort == options.Value.Port)
            return true;

        var token = context.Request.Headers.Authorization.ToString();

        if (!token.Contains("Bearer "))
            return false;

        token = token.Replace("Bearer ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Trim();

        if (token.ToString() != options.Value.MainPortAccessToken)
            return false;

        return true;
    }

    private async Task MeasureHealth(HttpContext context)
    {
        var route = options.Value.Path;
        Activity.Current?.SetTag("http.route", route);
        Activity.Current?.SetTag("url.path", context.Request.Path);

        var logger = context.RequestServices
            .GetRequiredService<ILogger<HealthCheckMiddleware>>();

        var healthCheckService = context.RequestServices
            .GetRequiredService<HealthCheckService>();

        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(options.Value.HealthCheckTimeoutSeconds));

        var report = await healthCheckService.CheckHealthAsync(cts.Token);

        if (report.Status == HealthStatus.Healthy)
        {
            logger.LogDebug("[Health: {status}] Health check enusred good service status.", report.Status);
            context.Response.StatusCode = 200;
        }
        else
        {
            logger.LogError("[Health: {status}] Health check failed indicates service problem.", report.Status);
            context.Response.StatusCode = 503;
        }

        await UIResponseWriter.WriteHealthCheckUIResponse(context, report);
    }
}
