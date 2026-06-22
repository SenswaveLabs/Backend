
using Npgsql;

namespace Senswave.TestInfrastructure.Fixtures.Database;

public class PostgreSqlFixture(string databaseName) : IDatabase
{

    public string GetDatabase() => databaseName;

    public string GetHostname() => "localhost";

    public string GetPassword() => "postgres";

    public string GetUsername() => "postgres";

    public int GetPort() => 5432;

    public bool IsWorking()
    {
        try
        {
            var connectionString = $"Host={GetHostname()};Port={GetPort()};Username={GetUsername()};Password={GetPassword()};Database=postgres";
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            connection.Close();
            connection.Dispose();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync() => Task.CompletedTask;
}
