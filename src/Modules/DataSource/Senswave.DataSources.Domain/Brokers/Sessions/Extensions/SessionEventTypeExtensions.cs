using Senswave.DataSources.Domain.Brokers.Sessions.Enums;

namespace Senswave.DataSources.Domain.Brokers.Sessions.Extensions;

public static class SessionEventTypeExtensions
{
    public static SessionEventType ToClientEventType(this string protocol) => protocol switch
    {
        "Connected" => SessionEventType.Connected,
        "Disconnected" => SessionEventType.Disconnected,
        "Reconnecting" => SessionEventType.Reconnecting,
        "FinishedByUser" => SessionEventType.FinishedByUser,
        "MessageIgnored" => SessionEventType.MessageIgnored,
        "MessagePublished" => SessionEventType.MessagePublished,
        "MessageReceived" => SessionEventType.MessageReceived,
        "FailedToPublishMessage" => SessionEventType.FailedToPublishMessage,
        _ => SessionEventType.Empty
    };

    public static string FromEventType(this SessionEventType protocol) => protocol switch
    {
        SessionEventType.Connected => "Connected",
        SessionEventType.Disconnected => "Disconnected",
        SessionEventType.Reconnecting => "Reconnecting",
        SessionEventType.FinishedByUser => "FinishedByUser",
        SessionEventType.MessageIgnored => "MessageIgnored",
        SessionEventType.MessagePublished => "MessagePublished",
        SessionEventType.MessageReceived => "MessageReceived",
        SessionEventType.FailedToPublishMessage => "FailedToPublishMessage",
        _ => "Empty",
    };
}
