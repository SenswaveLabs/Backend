namespace Senswave.Homes.Application.Homes.Features.GetHome;

public class GetHomeErrors
{
    public static Error HomeNotFound => Error.NotFound("HomeNotFound", "Home not found in the database");
}