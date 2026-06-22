namespace Senswave.DataSources.Application.Brokers.Sessions.GetSessions;

public class GetSessionsErrors
{
    public static Error SessionsNotFound => Error.NotFound("SessionsNotFound", "No sessions found.");
}
