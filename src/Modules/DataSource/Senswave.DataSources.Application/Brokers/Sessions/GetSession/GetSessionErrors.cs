namespace Senswave.DataSources.Application.Brokers.Sessions.GetSession;

public static class GetSessionErrors
{
    public static Error SessionNotFound => Error.NotFound("SessionNotFound", "The session was not found.");
}
