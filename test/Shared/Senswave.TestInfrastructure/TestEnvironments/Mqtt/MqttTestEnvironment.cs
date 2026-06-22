using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Senswave.Api;
using Senswave.Api.RateLimiters.Anonymous;
using Senswave.Api.RateLimiters.User;
using Senswave.Automations.Domain.Options;
using Senswave.DataSources.Domain.Brokers.Brokers.Options;
using Senswave.Devices.Domain.Devices.Options;
using Senswave.Homes.Domain;
using Senswave.Infrastructure.Persistence;
using Senswave.TestInfrastructure.Extensions;
using Senswave.TestInfrastructure.Fixtures.Database;
using Senswave.TestInfrastructure.Fixtures.MessageBus;
using Senswave.TestInfrastructure.Fixtures.Mqtt;
using Senswave.TestInfrastructure.TestSetup.Initializers;

namespace Senswave.TestInfrastructure.TestEnvironments.Mqtt;

public class MqttTestEnvironment : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly static SemaphoreSlim _synchronizer = new(1, 1);

#if TESTS
    internal static readonly IDatabase Database = new PostgreSqlFixture("SenswaveMqttEndTests");
    internal static readonly IMessageBus MessageBus = new RabbitMqFixture();
#else
    internal static readonly IDatabase Database = new PostgreSqlContainerFixture("SenswaveMqttEndTests");
    internal static readonly IMessageBus MessageBus = new RabbitMqContainerFixture();
#endif
    internal static readonly IMqttFixture MqttFixture = new MosquittoContainerFixture();

    public IMqttFixture GetMqttProvider() => MqttFixture;

    #region Options

    public static AutomationOptions AutomationOptions => new()
    {
        Limits = new()
        {
            AutomationsPerHome = 16,
        }
    };

    public static DevicesOptions DevicesOptions => new()
    {
        Limits = new()
        {
            DevicesPerHome = 100,
            DashboardsPerDevice = 4,
            OperationsPerDevice = 100,
            OperationValuesPerOperation = 4
        }
    };

    public static readonly BrokerOptions BrokerOptions = new()
    {
        Limits = new()
        {
            Brokers = 10000
        }
    };

    public static readonly HomeModuleOptions HomeOptions = new()
    {
        Limits = new()
        {
            Homes = 10000,
            RoomsPerHome = 10000,
            UsersPerHome = 10000
        }
    };

    public static readonly AnonymousRateLimiterOptions AnonymousRateLimiterOptions = new()
    {
        ReplenishmentPeriodSeconds = 1,
        TokenLimit = 100,
        QueueLimit = 0,
        TokensPerPeriod = 100
    };

    public static readonly UserRateLimiterOptions UserRateLimiterOptions = new()
    {
        ReplenishmentPeriodSeconds = 1,
        TokenLimit = 100,
        QueueLimit = 0,
        TokensPerPeriod = 100
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

            services.ReplaceOptions(AnonymousRateLimiterOptions);
            services.ReplaceOptions(UserRateLimiterOptions);
        });
    }

    public async Task InitializeAsync()
    {
        await _synchronizer.WaitAsync();

        try
        {
            await Database.InitializeAsync();
            await MessageBus.InitializeAsync();
            await MqttFixture.InitializeAsync();

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

            if (!MqttFixture.IsWorking())
            {
                //If running locally start rabbitmq container or change running to release
                throw new InvalidOperationException("Mqtt container is not running.");
            }

            using var scope = Services.CreateScope();

            var databaseContextInitializer = scope.ServiceProvider.GetRequiredService<DatabaseContextInitializer>();
            await databaseContextInitializer.Initialize(CancellationToken.None);

            var userInitializer = new TestUsersInitializer(scope);
            await userInitializer.InitializeUsers();
        }
        finally
        {
            _synchronizer.Release();
        }
    }

    public new Task DisposeAsync() => Task.CompletedTask;
}
