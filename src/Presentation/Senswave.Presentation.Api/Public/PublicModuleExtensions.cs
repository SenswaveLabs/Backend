using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Senswave.Api.Public;

public static class PublicModuleExtensions
{
    public static IServiceCollection AddPublicModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<VersionOptions>(configuration.GetSection(VersionOptions.SectionName));
        return services;
    }

    public static IEndpointRouteBuilder MapApiPublicEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/.well-known/assetlinks.json", () =>
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", ".well-known", "assetlinks.json");
            return Results.File(path, "application/json", "assetlinks.json");

        }).WithTags(PublicModule.ModuleName)
           .WithName("Get Android Asset link")
           .WithMetadata(new AllowAnonymousAttribute());

        endpoints.MapGet("/.well-known/apple-app-site-association", () =>
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", ".well-known", "apple-app-site-association");
            return Results.File(path, "application/json", "apple-app-site-association");

        }).WithTags(PublicModule.ModuleName)
          .WithName("Get Android Association")
          .WithMetadata(new AllowAnonymousAttribute());

        endpoints.MapGet("api/version", (IOptionsSnapshot<VersionOptions> options) =>
        {
            var versionInfo = new
            {
                version = options.Value.Version
            };

            return Results.Ok(versionInfo);
        }).WithTags(PublicModule.ModuleName)
          .WithName("Get API Version")
          .WithMetadata(new AllowAnonymousAttribute());

        return endpoints;
    }

}
