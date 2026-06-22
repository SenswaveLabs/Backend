namespace Senswave.DataSources.Application.Brokers.Brokers.Features.DeleteSubscribtion;

public sealed class DeleteSubscribtionCommand : ICommand
{
    public Guid SubscriptionId { get; set; } = Guid.Empty;
    public Guid BrokerId { get; set; } = Guid.Empty;
    public Guid UserId { get; set; } = Guid.Empty;
}
