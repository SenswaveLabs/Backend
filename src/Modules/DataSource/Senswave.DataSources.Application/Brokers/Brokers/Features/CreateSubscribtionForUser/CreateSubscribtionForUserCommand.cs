namespace Senswave.DataSources.Application.Brokers.Brokers.Features.CreateSubscribtionForUser;

public sealed class CreateSubscribtionForUserCommand : ICommand<Guid>
{
    public Guid UserId { get; set; }

    public Guid BrokerId { get; set; }

    public string Topic { get; set; } = string.Empty;
}
