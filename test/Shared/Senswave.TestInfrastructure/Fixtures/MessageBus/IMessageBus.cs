namespace Senswave.TestInfrastructure.Fixtures.MessageBus;

public interface IMessageBus : IAsyncLifetime
{
    string GetConnectionString();
    string GetUsername();
    string GetPassword();
    bool IsWorking();
}
