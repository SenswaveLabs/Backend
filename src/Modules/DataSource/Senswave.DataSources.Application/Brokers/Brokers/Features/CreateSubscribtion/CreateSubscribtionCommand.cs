namespace Senswave.DataSources.Application.Brokers.Brokers.Features.CreateSubscribtion;

public sealed class CreateSubscribtionCommand : ICommand<Guid>
{
    public bool FailWhenTopicAlreadyExists { get; set; } = true;

    public Guid BrokerId { get; set; }

    public string Topic { get; set; } = string.Empty;
}
