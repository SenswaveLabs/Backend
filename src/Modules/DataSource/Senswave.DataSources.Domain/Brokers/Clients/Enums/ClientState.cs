namespace Senswave.DataSources.Domain.Brokers.Clients.Enums;

public enum ClientState
{
    Invalid = -1,
    Empty = 0,

    NotStarted = 1,
    Working = 2,
    Restarting = 3,
    Stopped = 4,
}
