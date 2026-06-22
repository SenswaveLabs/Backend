using Senswave.Abstractions.Resulting;

namespace Senswave.LiveUpdates.Api.Services.RateLimiter;

internal class RateLimiterErrors
{
    public static Error TooManyRequests = Error.ServerError("TooManyRequests", "Rate limit exceeded. Please try again later.");

    public static Error ConnectionNotFound = Error.NotFound("ConnectionNotFound", "The specified connection ID was not found in the rate limiter.");
}
