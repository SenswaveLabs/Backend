using MassTransit;
using Senswave.Infrastructure.Diagnostics.OpenTelemetry;
using Senswave.Infrastructure.Persistence;
using Senswave.Infrastructure.Web.Endpoints;
using Senswave.Users.Domain.Interfaces;
using Serilog;

namespace Senswave.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var app = BuildHost(args);

        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(20));

            logger.LogInformation("Initializing application...");

            var endpoints = scope.ServiceProvider.GetRequiredService<IEnumerable<IEndpoint>>();

            foreach (var endpoint in endpoints)
                logger.LogDebug("Action: {action}", endpoint.GetType());

            var databaseContextInitializer = scope.ServiceProvider.GetRequiredService<DatabaseContextInitializer>();
            await databaseContextInitializer.Initialize(cancellationTokenSource.Token);

            var legalInitializer = scope.ServiceProvider.GetRequiredService<ILegalService>();
            await legalInitializer.VerifyLegal(cancellationTokenSource.Token);

            logger.LogInformation("Starting application...");

            await app.RunAsync();

            logger.LogInformation("Application stopped. See ya!");
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Application failed to start.");
        }
    }

    private static IHost BuildHost(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
            .UseSerilog((context, configuration) =>
            {
                configuration.ReadFrom
                    .Configuration(context.Configuration);

                var openTelemetryConfiguration = context.Configuration
                    .GetSection(OpenTelemetryOptions.SectionName)
                    .Get<OpenTelemetryOptions>();

                if (openTelemetryConfiguration?.Enabled ?? false)
                {
                    var url = context.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];

                    if (string.IsNullOrEmpty(url))
                        throw new ConfigurationException("Invalid url for OpenTelemetry");

                    configuration.WriteTo.OpenTelemetry(opts =>
                    {
                        opts.Endpoint = url;
                        opts.ResourceAttributes = new Dictionary<string, object>
                        {
                            ["service.name"] = context.Configuration["OTEL_SERVICE_NAME"]!
                        };
                    });
                }
            });

        return builder.Build();
    }
}
