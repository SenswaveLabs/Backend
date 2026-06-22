namespace Senswave.Integration.DataSource.BrokerConnection.Notifications;

public class SubscribeNotification
{
    public Guid BrokerId { get; set; }
    public string Topic { get; set; } = string.Empty;
}
