namespace Senswave.Integration.DataSource.BrokerConnection.Notifications;

public class UnsubscribeNotifications
{
    public Guid BrokerId { get; set; }
    public string Topic { get; set; } = string.Empty;
}
