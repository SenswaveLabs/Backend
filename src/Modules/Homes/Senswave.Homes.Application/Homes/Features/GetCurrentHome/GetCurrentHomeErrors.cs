namespace Senswave.Homes.Application.Homes.Features.GetCurrentHome;

public class GetCurrentHomeErrors
{
    public static Error HomeNotFound => Error.NotFound("HomeNotFound", "Failed to deduce current home for user.");
}