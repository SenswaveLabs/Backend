namespace Senswave.Homes.Application.Homes.Features.CreateHome;

public class CreateHomeErrors
{
    public static Error NameAlreadyUsed
        => Error.Conflict("NameAlreadyUsed", "Name already used by other home.");

    public static Error LimitOfHomesReached
        => Error.Conflict("LimitOfHomesReached", "Limit of homes reached.");
}