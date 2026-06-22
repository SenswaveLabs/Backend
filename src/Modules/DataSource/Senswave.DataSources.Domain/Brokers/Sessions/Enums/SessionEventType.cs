namespace Senswave.DataSources.Domain.Brokers.Sessions.Enums;

public enum SessionEventType
{
    Invalid = -1,
    Empty = 0,

    Connected = 1,
    Disconnected = 2,
    Reconnecting = 3,
    FinishedByUser = 4,

    MessageReceived = 10,
    MessageIgnored = 11,
    MessagePublished = 12,
    FailedToPublishMessage = 13,
}
