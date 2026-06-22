using Serilog;

namespace Senswave.Presentation.Seed;

public class Program
{
    public static async Task Main(string[] args)
    {
        var app = BuildHost(args);

        await app.RunAsync();
    }

    public static IHost BuildHost(string[] args) => Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
        .UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration))
        .Build();
}
