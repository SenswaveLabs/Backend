using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Senswave.Api;
using Senswave.DataSources.Domain.Brokers.Brokers.Entities;
using Senswave.DataSources.Domain.Brokers.Sessions.Entities;
using Senswave.DataSources.Domain.Brokers.Sessions.Enums;
using Senswave.DataSources.Infrastructure;
using Senswave.Devices.Domain.Operations.ValueObjects;
using Senswave.Devices.Infrastructure;
using Senswave.TestInfrastructure.TestEnvironments.Common;
using Senswave.TestInfrastructure.TestSetup.Initializers;
using Senswave.TestInfrastructure.TestSetup.Models;
using Senswave.TestInfrastructure.TestSetup.Models.Automations;
using Senswave.TestInfrastructure.TestSetup.Models.Homes;
using Senswave.TestInfrastructure.TestSetup.Models.Users;
using Senswave.Users.Domain.ValueObjects;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Senswave.TestInfrastructure.TestEnvironments.Base;

public class BaseFeatureTest(WebApplicationFactory<Program> factory) : IClassFixture<BaseTestEnvironment>
{
    #region Privileges

    public static readonly List<object[]> HomePriviliges =
    [
        [_managePrivilege],
        [_displayPrivilege]
    ];

    public static readonly List<object[]> DevicePrivileges =
    [
        [_displayPrivilege],
        [_actionPrivilege],
        [_managePrivilege]
    ];

    public static IEnumerable<object[]> NotManageDevicePrivileges =>
    [
        [_displayPrivilege],
        [_actionPrivilege],
    ];

    public static IEnumerable<object[]> DisplayDevicePrivileges => DevicePrivileges.GetRange(0, 3);
    public static IEnumerable<object[]> InvokeDevicePrivileges => DevicePrivileges.GetRange(1, 2);
    public static IEnumerable<object[]> ManageDevicePrivileges => DevicePrivileges.GetRange(2, 1);

    protected const string _managePrivilege = "Manage";
    protected const string _displayPrivilege = "Display";
    protected const string _actionPrivilege = "Action";

    #endregion

    protected WebApplicationFactory<Program> Factory => factory;

    protected IServiceProvider Services => Factory.Server.Services;

    protected HttpClient CreateUnauthorizedClient(bool allowRedirect = true)
    {
        if (allowRedirect)
            return Factory.CreateClient();

        var options = new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        };

        return Factory.CreateClient(options);
    }

    protected async Task<HttpClient> CreateAdmin()
    {
        var client = CreateUnauthorizedClient();
        await AuthorizeClientAsAdmin(client);
        return client;
    }

    protected async Task<HttpClient> CreateUser()
    {
        var client = CreateUnauthorizedClient();
        await AuthorizeClientAsUser(client);
        return client;
    }

    protected async Task<HttpClient> CreateIntruder()
    {
        var client = CreateUnauthorizedClient();
        await AuthorizeClientAsIntruder(client);
        return client;
    }

    protected async Task<(HttpClient, string, string)> CreateClientWithConsent()
    {
        var email = $"{Guid.NewGuid()}@gmail.com";
        var password = "SuperSecure123455!@#^%&";

        var scope = Factory.Services.CreateScope();
        var initializer = new TestUsersInitializer(scope);
        await initializer.InitializeUsers(
        [
            new TestUser
            {
                Email = email,
                Password = password,
                Role = RoleTypes.User
            }
        ]);

        var client = CreateUnauthorizedClient();
        await Tests.AuthorizeClientWithConsent(client, email, password);
        return (client, email, password);
    }

    protected async Task<(HttpClient, string, string)> CreateClientWithRoleWithoutConsent()
    {
        var email = $"{Guid.NewGuid()}@gmail.com";
        var password = "SuperSecure123455!@#^%&";

        var scope = Factory.Services.CreateScope();
        var initializer = new TestUsersInitializer(scope);
        await initializer.InitializeUsers(
        [
            new TestUser
            {
                Email = email,
                Password = password,
                Role = RoleTypes.User
            }
        ]);

        var client = CreateUnauthorizedClient();
        await Tests.AuthorizeClient(client, email, password);
        return (client, email, password);
    }

    protected HttpMessageHandler CreateHandler() => Factory.Server.CreateHandler();

    protected async Task UpdateSetting(string keyPath, object newValue)
    {
        var baserPath = Factory.Server.Services.GetService<IHostEnvironment>();
        var filePath = Path.Combine(baserPath!.ContentRootPath, "appsettings.Test.json");
        var json = await File.ReadAllTextAsync(filePath);

        var root = JsonNode.Parse(json)!;

        var parts = keyPath.Split(':');
        JsonNode? current = root;

        for (int i = 0; i < parts.Length - 1; i++)
        {
            current = (current?[parts[i]]) ?? throw new ArgumentNullException(nameof(current));
        }

        var finalKey = parts[^1];

        if (current is JsonObject obj && obj.ContainsKey(finalKey))
        {
            obj[finalKey] = JsonValue.Create(newValue);
        }

        var opts = new JsonSerializerOptions(JsonSerializerDefaults.General)
        {
            WriteIndented = true
        };

        var payload = JsonSerializer.Serialize(root, opts);

        await File.WriteAllTextAsync(
            filePath,
            payload
        );
    }

    #region Authorization

    protected static async Task AuthorizeClientAsAdmin(HttpClient client)
        => await Tests.AuthorizeClientWithConsent(client, "admin@gmail.com", "Admin123456!");

    protected static async Task AuthorizeClientAsIntruder(HttpClient client)
        => await Tests.AuthorizeClientWithConsent(client, "intruder@gmail.com", "Intruder123!");

    protected static async Task AuthorizeClientAsUser(HttpClient client)
        => await Tests.AuthorizeClientAsUser(client);

    protected async Task<HttpClient> AuthorizedUser()
    {
        var client = CreateUnauthorizedClient();
        await AuthorizeClientAsUser(client);
        return client;
    }

    protected async Task<HttpClient> AuthorizedAdmin()
    {
        var client = CreateUnauthorizedClient();
        await AuthorizeClientAsAdmin(client);
        return client;
    }

    protected async Task<HttpClient> AuthorizedIntruder()
    {
        var client = CreateUnauthorizedClient();
        await AuthorizeClientAsIntruder(client);
        return client;
    }

    #endregion

    #region DataSource
    protected async Task<Guid> PostBroker() => await Tests.PostBroker(Factory.CreateClient());

    /// <summary>
    /// Sessions are created internally and cannot be posted (starting with client starting).
    /// So it can be created only with mqtt tests.
    /// </summary>
    /// <returns></returns>
    protected async Task<Guid> PrepareSessionWithLogs(Guid brokerId)
    {
        using var scope = Factory.Server.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<DataSourcesContext>();

        var broker = await context.Brokers.FindAsync(brokerId);

        var session = new Session
        {
            Broker = broker!,
            Logs = [],
            Finished = false,

            UpdatedAtUtc = DateTime.UtcNow,
            CreatedAtUtc = DateTime.UtcNow,
        };

        context.Sessions.Add(session);


        var logs = new List<Log>
        {
            new()
            {
                Session = session,

                Type = SessionEventType.Connected,
                Data = string.Empty,

                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow,
            },
            new()
            {
                Session = session,

                Type = SessionEventType.Reconnecting,
                Data = "{\"Reason\":\"ServerBusy\"}",

                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow,
            }
        };

        foreach (var log in logs)
            session.Logs.Add(log);

        context.Sessions.Add(session);

        await context.SaveChangesAsync();

        return session.Id;
    }

    protected async Task<Guid> PrepareSubscribtion(Guid brokerId, string? topic = null)
    {
        using var scope = Factory.Server.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<DataSourcesContext>();

        var broker = await context.Brokers.FindAsync(brokerId);

        var subscribtion = new Subscribtion
        {
            Broker = broker!,
            Topic = topic ?? $"test/{Guid.NewGuid()}",
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
        };

        context.Subscribtions.Add(subscribtion);
        await context.SaveChangesAsync();

        return subscribtion.Id;
    }

    protected async Task<Guid> FinishSession(Guid sessionId)
    {
        using var scope = Factory.Server.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<DataSourcesContext>();

        var session = await context.Sessions
            .Where(x => x.Id == sessionId)
            .FirstAsync();

        var logs = new Log()
        {
            Session = session!,

            Type = SessionEventType.Disconnected,
            Data = "{\"Reason\":\"ServerBusy\"}",

            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
        };

        session.Logs.Add(logs);
        session.Finished = true;
        context.Logs.Add(logs);
        await context.SaveChangesAsync();

        return session.Id;
    }

    #endregion

    #region Homes

    protected async Task<Guid> PostHome(int longitude = 76, int latitude = 51)
        => await Tests.PostHome(Factory.CreateClient(), longitude, latitude);

    protected async Task<Guid> PostAdminHome(int longitude = 76, int latitude = 51)
    => await Tests.PostAdminHome(Factory.CreateClient(), longitude, latitude);

    protected async Task<Guid> PutBrokerForHome(Guid brokerId, Guid homeId)
        => await Tests.PutBrokerForHome(Factory.CreateClient(), brokerId, homeId);


    protected async Task<Guid> PutBrokerForHome(Guid homeId)
    {
        var brokerId = await PostBroker();
        return await Tests.PutBrokerForHome(Factory.CreateClient(), brokerId, homeId);
    }

    protected async Task<Guid> PostHomeWithBroker()
    {
        var brokerId = await PostBroker();
        var homeId = await PostHome();
        await PutBrokerForHome(brokerId, homeId);
        return homeId;
    }

    protected async Task<Guid> PostRoom(Guid homeId)
        => await Tests.PostRoom(Factory.CreateClient(), homeId);

    #endregion

    #region Devices

    protected async Task<Guid> PostDevice(Guid homeId, Guid roomId = default)
        => await Tests.PostDevice(Factory.CreateClient(), homeId, roomId);

    protected async Task PatchDeviceWithSwitchTile(Guid deviceId, Guid operationId)
        => await Tests.AssingSwitchTile(Factory.CreateClient(), deviceId, operationId);

    protected async Task PatchDeviceWithDisplayTile(Guid deviceId, Guid operationId)
        => await Tests.AssingDisplayTile(Factory.CreateClient(), deviceId, operationId);

    protected async Task<Guid> PostDashboard(Guid deviceId)
        => await Tests.PostDashboard(Factory.CreateClient(), deviceId);

    protected async Task SetWidgetOnDashboard(Guid dashboardId, Guid widgetId)
        => await Tests.SetWidgetOnDashboard(Factory.CreateClient(), dashboardId, widgetId);

    protected async Task<Guid> PostBooleanOperation(Guid deviceId)
        => await Tests.PostBooleanOperation(Factory.CreateClient(), deviceId, true);

    protected async Task<Guid> PostHexColorOperation(Guid deviceId)
        => await Tests.PostHexColorOperation(Factory.CreateClient(), deviceId);

    protected async Task<Guid> PostIntegerOperation(Guid deviceId)
        => await Tests.PostIntegerOperation(Factory.CreateClient(), deviceId);

    protected async Task<Guid> PostIntegerRangedOperation(Guid deviceId)
        => await Tests.PostIntegerRangedOperation(Factory.CreateClient(), deviceId);

    protected async Task<Guid> PostNumberOperation(Guid deviceId)
        => await Tests.PostNumberOperation(Factory.CreateClient(), deviceId);

    protected async Task<Guid> PostOptionsOperation(Guid deviceId)
        => await Tests.PostOptionsOperation(Factory.CreateClient(), deviceId);

    protected async Task<Guid> PostNumberRangedOperation(Guid deviceId)
        => await Tests.PostNumberRangedOperation(Factory.CreateClient(), deviceId);

    protected Task<Guid> PostTextOperation(Guid guid)
        => Tests.PostTextOperation(Factory.CreateClient(), guid);

    protected async Task<Guid> PostButtonWidget(Guid operationId)
        => await Tests.CreateButtonWidget(Factory.CreateClient(), operationId, new() { ["value"] = true });

    protected async Task<Guid> PostSwitchWidget(Guid operationId)
        => await Tests.CreateSwitchWidget(Factory.CreateClient(), operationId);

    protected async Task<Guid> PostSliderWidget(Guid operationId)
        => await Tests.CreateSliderWidget(Factory.CreateClient(), operationId);

    protected async Task<Guid> PostColorWidget(Guid operationId)
        => await Tests.PostColorWidget(Factory.CreateClient(), operationId);

    protected async Task<Guid> PostRadioWidget(Guid operationId)
        => await Tests.CreateRadioWidget(Factory.CreateClient(), operationId);

    protected async Task<Guid> PostTimeSeriesGraphWidget(Guid operationId)
        => await Tests.CreateTimeSeriesGraphWidget(Factory.CreateClient(), operationId);

    protected async Task PatchDeviceWithBooleanPresence(Guid deviceId, Guid presenceOperationId)
    {
        var client = Factory.CreateClient();
        await Tests.AuthorizeClientAsUser(client);
        var request = RequestFactory.CreatePatchDevice(presenceType: "BooleanOperation", presenceOperationId: presenceOperationId);
        var json = JsonSerializer.Serialize(request);
        await client.PatchAsync($"{Paths.DevicesPath}/{deviceId}", new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
    }

    protected async Task SetPresenceOperationValue(Guid operationId, bool value, DateTime? at = null)
    {
        using var scope = Factory.Server.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DevicesContext>();
        var operation = await context.Operations.FirstAsync(x => x.Id == operationId);
        operation.Values.Add(new OperationValue
        {
            Operation = operation,
            InternalValue = new JsonObject { ["value"] = value },
            ProcessedAtUtc = at ?? DateTime.UtcNow
        });
        await context.SaveChangesAsync();
    }

    #endregion

    #region Sharing

    protected async Task<InviteFriendResponse> InviteFriend(Guid homeId, string friendEmail = "admin@gmail.com",
        string sharingType = "Manage")
        => await Tests.InviteFriends(
            Factory.CreateClient(),
            RequestFactory.CreateFriendInvitation(homeId, friendEmail, sharingType));

    protected static async Task AcceptInvitation(HttpClient acceptor, string password)
        => await Tests.AcceptInvitation(acceptor, password);

    protected async Task PrepareHomeSharingForAdmin(Guid homeId, string sharingType = "Manage")
    {
        var passwordResponse = await InviteFriend(homeId, "admin@gmail.com", sharingType);
        var friend = CreateUnauthorizedClient();
        await AuthorizeClientAsAdmin(friend);
        await AcceptInvitation(friend, passwordResponse.Password);
    }

    protected async Task PrepareDeviceSharing(string shraingType, Guid deviceId)
        => await Tests.PrepareDeviceSharings(CreateUnauthorizedClient(), "admin@gmail.com", shraingType, deviceId);

    protected static async Task<IList<Guid>> GetDeviceSharings(HttpClient authorizedClient, Guid device)
        => await Tests.GetDeviceSharings(authorizedClient, device);

    #endregion

    #region Automations

    protected async Task<Guid> PostAutomation(Guid homeId, IList<PostConditionItemRequest> conditions, IList<PostResultItemRequest> results)
        => await Tests.PostAutomation(Factory.CreateClient(), homeId, conditions, results);

    protected static PostConditionItemRequest CreateCondition(Guid operationId, string conditionType = "BooleanCondition",
        JsonObject? conditionConfig = null)
    {
        conditionConfig ??= new JsonObject { ["isOn"] = true };

        return new PostConditionItemRequest
        {
            OperationId = operationId,
            ConditionType = conditionType,
            ConditionConfiguration = conditionConfig
        };

    }

    protected static PostResultItemRequest CreateResult(Guid operationId, JsonValue? valueToSend = null) => new()
    {
        OperationId = operationId,
        ValueToSend = valueToSend ?? JsonValue.Create(string.Empty)
    };

    #endregion

    #region SetUp

    protected async Task<BaseUserEntities> SetUpTestEntities()
    {
        var home = await PostHome();
        var broker = await PostBroker();
        await PutBrokerForHome(broker, home);
        var device = await PostDevice(home);
        var conditionOperation = await PostBooleanOperation(device);
        var resultOperation = await PostNumberOperation(device);
        var automation = await PostAutomation(
            home, [CreateCondition(conditionOperation)], [CreateResult(resultOperation)]);

        return new BaseUserEntities
        {
            HomeId = home,
            BrokerId = broker,
            DeviceId = device,
            OperationId1 = conditionOperation,
            OperationId2 = resultOperation,
            AutomationId = automation
        };
    }



    #endregion
}
