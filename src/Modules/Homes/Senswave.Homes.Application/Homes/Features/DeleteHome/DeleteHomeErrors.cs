namespace Senswave.Homes.Application.Homes.Features.DeleteHome;

public class DeleteHomeErrors
{
    public static Error HomeNotFound => Error.NotFound("HomeNotFound", "Home not found in the database");

}