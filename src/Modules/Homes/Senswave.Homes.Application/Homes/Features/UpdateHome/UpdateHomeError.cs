namespace Senswave.Homes.Application.Homes.Features.UpdateHome;

public static class UpdateHomeError
{
    public static Error HomeNotFound => Error.NotFound("HomeNotFound", "Home not found in the database");

    public static Error HomeNameUsed => Error.NotFound("HomeNameUsed", "Your home with same name found in database");
}