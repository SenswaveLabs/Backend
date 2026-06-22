namespace Senswave.Homes.Application.Sharings.Features.GetSharings;

public static class GetSharingsErrors
{
    public static Error HomeNotFound =>
        Error.NotFound("HomeNotFound", "Home with provided id not found in the database");
}