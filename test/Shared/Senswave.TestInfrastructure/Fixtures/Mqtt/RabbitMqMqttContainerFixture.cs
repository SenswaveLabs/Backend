using Senswave.TestInfrastructure.Fixtures.MessageBus;
using Testcontainers.RabbitMq;

namespace Senswave.TestInfrastructure.Fixtures.Mqtt;

public class RabbitMqMqttContainerFixture : RabbitMqContainerFixture, IMqttFixture
{
    public string Hostname => "localhost";
    public int Port => 1883;
    public bool UseTls => false;
    public string Version => "MqttV310";

    public string Password => Password;
    public string Username => Username;

    public RabbitMqMqttContainerFixture()
    {
        var builder = new RabbitMqBuilder("rabbitmq:latest")
            .WithPortBinding(15672, true)
            .WithPortBinding(5672, true)
            .WithPortBinding(1883, true)
            .WithPassword(Password)
            .WithUsername(Username);

#if DEBUG
        builder.WithReuse(true);
#endif

        Container = builder.Build();
    }


    public async new Task InitializeAsync()
    {
        await Container.StartAsync();
        await Container.ExecAsync(["rabbitmq-plugins", "enable", "rabbitmq_mqtt"]);
        await Container.StopAsync();
        await Container.StartAsync();
    }
}
