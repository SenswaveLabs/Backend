namespace Senswave.TestInfrastructure.Fixtures.Database;

public interface IDatabase : IAsyncLifetime
{
    int GetPort();
    string GetUsername();
    string GetHostname();
    string GetPassword();
    string GetDatabase();
    bool IsWorking();
}
