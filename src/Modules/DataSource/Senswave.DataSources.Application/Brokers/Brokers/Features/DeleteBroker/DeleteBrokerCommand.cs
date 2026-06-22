namespace Senswave.DataSources.Application.Brokers.Brokers.Features.DeleteBroker;

public class DeleteBrokerCommand : ICommand
{
    public Guid BrokerId { get; set; } = Guid.Empty;
    public Guid UserId { get; set; } = Guid.Empty;
}