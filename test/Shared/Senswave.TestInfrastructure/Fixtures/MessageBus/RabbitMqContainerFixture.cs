using DotNet.Testcontainers.Containers;
using Testcontainers.RabbitMq;

namespace Senswave.TestInfrastructure.Fixtures.MessageBus;

public class RabbitMqContainerFixture : IMessageBus
{
    public RabbitMqContainer Container;

    private static string Username => "testguest";
    private static string Password => "testguest";

    public RabbitMqContainerFixture()
    {
        var builder = new RabbitMqBuilder("rabbitmq:4.1.3-management")
            .WithPortBinding(15672, true)
            .WithPortBinding(5672, true)
            .WithPassword(Password)
            .WithUsername(Username);

        Container = builder.Build();
    }

    public string GetConnectionString()
        => Container.GetConnectionString();

    public string GetUsername() => Username;

    public string GetPassword() => Password;

    public bool IsWorking() => Container.State == TestcontainersStates.Running;

    public async Task InitializeAsync()
    {
        await Container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await Container.StopAsync();
    }
}
