using Microsoft.AspNetCore.RateLimiting;
using Senswave.Api.RateLimiters.Anonymous;
using Senswave.Api.RateLimiters.User;
using System.Threading.RateLimiting;

namespace Senswave.Api.RateLimiters;

public static class RateLimiterExtensions
{
    public static ValueTask AcceptRejectRule(OnRejectedContext context, CancellationToken cancellationToken)
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter =
                ((int)retryAfter.TotalSeconds).ToString();
        }

        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        return new();
    }

    public static IServiceCollection AddRateLimiters(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AnonymousRateLimiterOptions>(
            configuration.GetSection(AnonymousRateLimiterOptions.Name));

        services.Configure<UserRateLimiterOptions>(
            configuration.GetSection(UserRateLimiterOptions.Name));

        services.Configure<RateLimitersOptions>(
            configuration.GetSection(RateLimitersOptions.Name));

        services.AddRateLimiter(options =>
        {
            // Annonymus User
            options.AddPolicy<string, AnonymousRateLimiter>(AnonymousRateLimiter.PolicyName);

            // Logged User
            options.AddPolicy<string, UserRateLimiter>(UserRateLimiter.PolicyName);
        });

        return services;
    }
}
