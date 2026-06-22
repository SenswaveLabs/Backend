using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
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
using Senswave.TestInfrastructure.TestSetup.Initializers;
using Senswave.Users.Application.Auth.Google.Services;
using Senswave.Users.Domain.Interfaces;

namespace Senswave.TestInfrastructure.TestEnvironments.Base;

public class BaseTestEnvironment : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly static SemaphoreSlim _synchronizer = new(1, 1);

#if TESTS
    internal static readonly IDatabase Database = new PostgreSqlFixture("SenswaveEndTests");
    internal static readonly IMessageBus MessageBus = new RabbitMqFixture();
#else
    internal static readonly IDatabase Database = new PostgreSqlContainerFixture("SenswaveEndTests");
    internal static readonly IMessageBus MessageBus = new RabbitMqContainerFixture();
#endif

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
            Brokers = 10000,
        },
        TestConnectionOnChange = false
    };

    public static readonly HomeModuleOptions HomeOptions = new()
    {
        Limits = new()
        {
            Homes = 10000,
            RoomsPerHome = 10000,
            UsersPerHome = 10000
        },
        Sharings = new()
        {
            InvitationExpiresInSeconds = 10
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

    #region ServicesMocks

    public Mock<IGoogleService> GoogleServiceMock { get; } = new();

    public Mock<IEmailService> EmailServiceMock { get; } = new();

    #endregion

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((ctx, config) =>
        {
            config.AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true);
        });

        builder.ConfigureTestServices(services =>
        {
            services.ReplaceRabbitMqBus(MessageBus);
            services.ReplacePostgreSql(Database);

            services.ReplaceOptions(AnonymousRateLimiterOptions);
            services.ReplaceOptions(UserRateLimiterOptions);

            services.ReplaceOptions(BrokerOptions);
            services.ReplaceOptions(HomeOptions);
            services.ReplaceOptions(DevicesOptions);
            services.ReplaceOptions(AutomationOptions);

            services.ReplaceService(GoogleServiceMock.Object);
            services.ReplaceService(EmailServiceMock.Object);
        });


        builder.UseEnvironment("Test");
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
