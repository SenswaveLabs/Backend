
using RabbitMQ.Client;

namespace Senswave.TestInfrastructure.Fixtures.MessageBus;

public class RabbitMqFixture : IMessageBus
{
    public string GetConnectionString() => "amqp://localhost:5672";
    public string GetUsername() => "guest";
    public string GetPassword() => "guest";
    public bool IsWorking()
    {
        try
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(GetConnectionString()),
                UserName = GetUsername(),
                Password = GetPassword(),
                ContinuationTimeout = TimeSpan.FromSeconds(2)
            };

            using var connection = factory.CreateConnectionAsync().Result;
            connection.CloseAsync();

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync() => Task.CompletedTask;
}
