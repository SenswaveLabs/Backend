namespace Senswave.Homes.Application.Homes.Features.GetHomes;

public static class GetHomesErrors
{
    public static Error HomesNotFound => Error.NotFound("HomesNotFound", "Homes not found in the database");
}