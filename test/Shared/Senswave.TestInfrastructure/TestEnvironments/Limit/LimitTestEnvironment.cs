using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Senswave.Api;
using Senswave.Automations.Domain.Options;
using Senswave.DataSources.Domain.Brokers.Brokers.Options;
using Senswave.Devices.Domain.Devices.Options;
using Senswave.Homes.Domain;
using Senswave.Infrastructure.Persistence;
using Senswave.TestInfrastructure.Extensions;
using Senswave.TestInfrastructure.Fixtures.Database;
using Senswave.TestInfrastructure.Fixtures.MessageBus;
using Senswave.TestInfrastructure.TestSetup.Initializers;
using Senswave.TestInfrastructure.TestSetup.Models.Users;
using Senswave.Users.Domain.ValueObjects;

namespace Senswave.TestInfrastructure.TestEnvironments.Limit;

public class LimitTestEnvironment : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly static SemaphoreSlim _synchronizer = new(1, 1);

#if TESTS
    internal static readonly IDatabase Database = new PostgreSqlFixture("SenswaveEndTestLimits");
    internal static readonly IMessageBus MessageBus = new RabbitMqFixture();
#else
    internal static readonly IDatabase Database = new PostgreSqlContainerFixture("SenswaveEndTestLimits");
    internal static readonly IMessageBus MessageBus = new RabbitMqContainerFixture();
#endif

    static LimitTestEnvironment()
    {
        Environment.SetEnvironmentVariable("Diagnostics__Health__Port", "8080");
    }

    #region Options

    public static AutomationOptions AutomationOptions => new()
    {
        Limits = new()
        {
            AutomationsPerHome = 1,
        }
    };

    public static DevicesOptions DevicesOptions => new()
    {
        Limits = new()
        {
            DevicesPerHome = 4,
            DashboardsPerDevice = 4,
            OperationsPerDevice = 5,
            OperationValuesPerOperation = 4
        }
    };

    public static readonly BrokerOptions BrokerOptions = new()
    {
        Limits = new()
        {
            Brokers = 2,
            InstanceMaxBrokerClients = 3, // kEEP IT Brokers + 1
            BrokerSubscribtions = 5
        },
        TestConnectionOnChange = false
    };

    public static readonly HomeModuleOptions HomeOptions = new()
    {
        Limits = new()
        {
            Homes = 2,
            RoomsPerHome = 2,
            UsersPerHome = 1
        }
    };

    #endregion

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {


        builder.ConfigureTestServices(services =>
        {
            services.ReplaceRabbitMqBus(MessageBus);
            services.ReplacePostgreSql(Database);

            services.ReplaceOptions(BrokerOptions);
            services.ReplaceOptions(HomeOptions);
            services.ReplaceOptions(DevicesOptions);
            services.ReplaceOptions(AutomationOptions);
        });
    }

    public async Task InitializeAsync()
    {
        await _synchronizer.WaitAsync();

        try
        {
            await Database.InitializeAsync();
            await MessageBus.InitializeAsync();

            if (!Database.IsWorking())
            {
                //If running locally start postgres container or change running to release
                throw new InvalidOperationException("PostgreSQL container is not running.");
            }

            if (!MessageBus.IsWorking())
            {
                //If running locally start rabbitmq container or change running to release
                throw new InvalidOperationException("RqbbitMq container is not running.");
            }

            using var scope = Services.CreateScope();

            var databaseContextInitializer = scope.ServiceProvider.GetRequiredService<DatabaseContextInitializer>();
            await databaseContextInitializer.Initialize(CancellationToken.None);

            TestUser[] TestUsers =
            [
                new TestUser
                {
                    Email = "user1@gmail.com",
                    Password = "User123456!",
                    Role = RoleTypes.User
                },
                new TestUser
                {
                    Email = "user2@gmail.com",
                    Password = "User123456!",
                    Role = RoleTypes.User
                }
            ];

            var userInitializer = new TestUsersInitializer(scope);
            await userInitializer.InitializeUsers(TestUsers);

        }
        finally
        {
            _synchronizer.Release();
        }
    }

    public new Task DisposeAsync() => Task.CompletedTask;
}
