using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using Senswave.DataSources.BrokerConnection.Interfaces;
using Senswave.DataSources.Infrastructure;
using Senswave.Devices.Domain.Operations.ValueObjects;
using Senswave.Devices.Infrastructure;
using Senswave.Homes.Infrastructure;
using Senswave.TestInfrastructure.Extensions;
using Senswave.TestInfrastructure.Fixtures.Mqtt;
using Senswave.TestInfrastructure.TestEnvironments.Base;
using Senswave.TestInfrastructure.TestEnvironments.Common;
using Senswave.TestInfrastructure.TestSetup.Initializers;
using Senswave.TestInfrastructure.TestSetup.Models;
using Senswave.TestInfrastructure.TestSetup.Models.Automations;
using Senswave.TestInfrastructure.TestSetup.Models.Common;
using Senswave.TestInfrastructure.TestSetup.Models.Homes;
using Senswave.TestInfrastructure.TestSetup.Models.Users;
using Senswave.Users.Domain.ValueObjects;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Senswave.TestInfrastructure.TestEnvironments.Mqtt;

public class MqttFeatureTest(MqttTestEnvironment factory) : IClassFixture<MqttTestEnvironment>
{

    #region Sharings

    public static List<object[]> CanActPriviliges => DevicePrivilegeList
            .Where(privilege => !privilege.Contains(_displayPrivilege))
            .ToList();

    public static IEnumerable<object[]> HomePriviliges => BaseFeatureTest.HomePriviliges;
    public static List<object[]> DevicePrivilegeList => BaseFeatureTest.DevicePrivileges;

    protected const string _displayPrivilege = "Display";
    protected const string _managePrivilege = "Manage";
    protected const string _invokePrivilege = "Action";

    #endregion

    protected MqttTestEnvironment Factory => factory;

    protected IMqttFixture MqttFixture => factory.GetMqttProvider();

    protected IServiceProvider Services => Factory.Server.Services;

    protected HttpClient CreateClient() => Factory.CreateClient();

    protected HttpMessageHandler CreateHandler() => Factory.Server.CreateHandler();

    #region Authorization

    protected HttpClient CreateUnauthorizedClient() => Factory.CreateClient();

    protected async Task<(HttpClient, string, string)> CreateClientWithConsent()
    {
        var email = $"{Guid.NewGuid()}@gmail.com";
        var password = "SuperSecure123455!@#^%&";

        var scope = Factory.Services.CreateScope();
        var initializer = new TestUsersInitializer(scope);
        await initializer.InitializeUsers(new[]
        {
            new TestUser
            {
                Email = email,
                Password = password,
                Role = RoleTypes.User
            }
        });

        var client = CreateUnauthorizedClient();
        await Tests.AuthorizeClientWithConsent(client, email, password);
        return (client, email, password);
    }

    protected static async Task AuthorizeClientAsAdmin(HttpClient client)
        => await Tests.AuthorizeClientWithConsent(client, "admin@gmail.com", "Admin123456!");

    protected static async Task AuthorizeClientAsUser(HttpClient client)
        => await Tests.AuthorizeClientAsUser(client);

    protected async Task<HttpClient> CreateAdmin()
    {
        var client = CreateClient();
        await AuthorizeClientAsAdmin(client);
        return client;
    }

    protected async Task<HttpClient> CreateUser()
    {
        var client = CreateClient();
        await AuthorizeClientAsUser(client);
        return client;
    }

    #endregion

    #region DataSource

    protected async Task DeleteBroker()
    {
        var client = Factory.CreateClient();
        using var scope = Factory.Services.CreateScope();
        var dataSourceContext = scope.ServiceProvider.GetRequiredService<DataSourcesContext>();

        var broker = await dataSourceContext.Brokers
            .Where(x => x.BrokerInfo.Port == MqttFixture.Port)
            .Where(x => x.BrokerInfo.Url == MqttFixture.Hostname)
            .FirstOrDefaultAsync();

        if (broker is null)
            return;

        await StopBrokerClientInternal(broker.Id);

        await Tests.DeleteBrokerNoCheks(Factory.CreateClient(), broker.Id);
    }

    protected async Task<Guid> PostMqttBroker(HttpClient client)
    {
        //Arrange
        using var scope = Factory.Services.CreateScope();
        var dataSourceContext = scope.ServiceProvider.GetRequiredService<DataSourcesContext>();

        var broker = await dataSourceContext.Brokers
            .Where(x => x.BrokerInfo.Url == MqttFixture.Hostname)
            .FirstOrDefaultAsync();

        if (broker is not null)
        {
            await StopBrokerClientInternal(broker.Id);

            dataSourceContext.Brokers.Remove(broker);
            await dataSourceContext.SaveChangesAsync();
        }

        var request = RequestFactory.CreatePostBrokerRequest(
            name: "Mqtt Broker",
            clientName: "Seed Connection",
            url: Factory.GetMqttProvider().Hostname,
            port: Factory.GetMqttProvider().Port,
            protocolVersion: Factory.GetMqttProvider().Version,
            useTls: Factory.GetMqttProvider().UseTls,
            password: Factory.GetMqttProvider().Password,
            username: Factory.GetMqttProvider().Username).Serialize();

        //Act
        var response = await client.PostAsync(Paths.BrokersPath, request);

        //Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var postResponse = JsonSerializer.Deserialize<IdResponse>(responseContent);
        Assert.NotNull(postResponse);
        Assert.False(postResponse!.Id == Guid.Empty);

        return postResponse!.Id;
    }

    protected async Task<Guid> PostMqttBroker()
    {
        //Arrange
        var client = Factory.CreateClient();
        using var scope = Factory.Services.CreateScope();
        var dataSourceContext = scope.ServiceProvider.GetRequiredService<DataSourcesContext>();

        var broker = await dataSourceContext.Brokers
            .Where(x => x.BrokerInfo.Url == MqttFixture.Hostname)
            .FirstOrDefaultAsync();

        if (broker is not null)
        {
            await StopBrokerClientInternal(broker.Id);

            dataSourceContext.Brokers.Remove(broker);
            await dataSourceContext.SaveChangesAsync();
        }

        var request = RequestFactory.CreatePostBrokerRequest(
            name: "Mqtt Broker",
            clientName: "Seed Connection",
            url: Factory.GetMqttProvider().Hostname,
            port: Factory.GetMqttProvider().Port,
            protocolVersion: Factory.GetMqttProvider().Version,
            useTls: Factory.GetMqttProvider().UseTls,
            password: Factory.GetMqttProvider().Password,
            username: Factory.GetMqttProvider().Username).Serialize();

        //Act
        await Tests.AuthorizeClientAsUser(client);
        var response = await client.PostAsync(Paths.BrokersPath, request);
        var responseContent = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.True(response.IsSuccessStatusCode, responseContent);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var postResponse = JsonSerializer.Deserialize<IdResponse>(responseContent);
        Assert.NotNull(postResponse);
        Assert.False(postResponse!.Id == Guid.Empty);

        return postResponse!.Id;
    }

    public async Task StartBrokerClient(Guid brokerId)
    {
        // Arrange
        await Task.Delay(1000);
        var client = CreateClient();
        var clientService = Services.GetRequiredService<IClientService>();

        var request = RequestFactory.CreateStartClient(
            username: Factory.GetMqttProvider().Username,
            password: Factory.GetMqttProvider().Password).Serialize();

        // Act
        var clientResult = clientService.GetClient(brokerId);

        if (clientResult.IsSuccess)
        {
            await clientService.StopClient(brokerId, CancellationToken.None);
        }

        await AuthorizeClientAsUser(client);

        for (int i = 0; i < 5; i++)
        {
            var response = await client.PostAsync(Paths.ClientsPath(brokerId), request);
            clientResult = clientService.GetClient(brokerId);

            if (clientResult.IsSuccess)
            {
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                Assert.True(clientResult.IsSuccess);
                return;
            }
        }

        Assert.Fail("Failed to connect with broker");
    }

    public async Task StartBrokerClient(HttpClient client, Guid brokerId)
    {
        // Arrange
        await Task.Delay(1000);
        var clientService = Services.GetRequiredService<IClientService>();

        var request = RequestFactory.CreateStartClient(
            username: Factory.GetMqttProvider().Username,
            password: Factory.GetMqttProvider().Password).Serialize();

        // Act
        var clientResult = clientService.GetClient(brokerId);

        if (clientResult.IsSuccess)
        {
            await clientService.StopClient(brokerId, CancellationToken.None);
        }

        for (int i = 0; i < 5; i++)
        {
            var response = await client.PostAsync(Paths.ClientsPath(brokerId), request);
            clientResult = clientService.GetClient(brokerId);

            if (clientResult.IsSuccess)
            {
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                Assert.True(clientResult.IsSuccess);
                return;
            }
        }

        Assert.Fail("Failed to connect with broker");
    }

    public async Task StopBrokerClientInternal(Guid brokerId)
    {
        // Arrange
        var clientService = Services.GetRequiredService<IClientService>();

        // Act
        await clientService.StopClient(brokerId, CancellationToken.None);
        await Task.Delay(2000);
    }

    #endregion

    #region Homes

    protected async Task<Guid> PostAndPutBrokerForHome(Guid dataSourceId)
    {
        using var scope = Factory.Services.CreateScope();
        var homeContext = scope.ServiceProvider.GetRequiredService<HomesContext>();

        var home = await homeContext.Homes
            .Where(x => x.DataSourceReference != null)
            .FirstOrDefaultAsync(x => x.DataSourceReference!.DataSourceId == dataSourceId);

        if (home != null)
        {
            await RemoveSharings(home.Id);
            return home.Id;
        }

        var homeId = await Tests.PostHome(Factory.CreateClient(), 75, 32);
        await Tests.PutBrokerForHome(Factory.CreateClient(), dataSourceId, homeId);
        return homeId;
    }

    protected async Task<Guid> PostRoom(Guid homeId)
        => await Tests.PostRoom(Factory.CreateClient(), homeId);

    protected async Task<InviteFriendResponse> InviteFriend(Guid homeId, string friendEmail = "admin@gmail.com",
        string sharingType = "Manage")
        => await Tests.InviteFriends(Factory.CreateClient(),
            RequestFactory.CreateFriendInvitation(homeId, friendEmail, sharingType));

    protected static async Task AcceptInvitation(HttpClient acceptor, string password)
        => await Tests.AcceptInvitation(acceptor, password);

    protected async Task PrepareHomeSharing(Guid homeId, string sharingType = "Manage")
    {
        var passwordResponse = await InviteFriend(homeId, "admin@gmail.com", sharingType);
        var friend = CreateClient();
        await AuthorizeClientAsAdmin(friend);
        await AcceptInvitation(friend, passwordResponse.Password);
    }

    protected async Task RemoveSharings(Guid homeId)
    {
        using var scope = Factory.Server.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<HomesContext>();

        var home = await context.HomeSharings
            .Where(x => x.HomeId == homeId)
            .ToListAsync();

        context.HomeSharings.RemoveRange(home);
        await context.SaveChangesAsync();
    }

    #endregion

    #region Devices

    protected async Task PrepareDeviceSharing(string shraingType, Guid deviceId)
        => await Tests.PrepareDeviceSharings(CreateClient(), "admin@gmail.com", shraingType, deviceId);

    protected async Task<Guid> PostDevice(Guid homeId, Guid roomId = default)
        => await Tests.PostDevice(Factory.CreateClient(), homeId, roomId);

    protected async Task PatchDeviceWithSwitchTile(Guid deviceId, Guid operationId)
        => await Tests.AssingSwitchTile(Factory.CreateClient(), deviceId, operationId);

    protected async Task PatchDeviceWithDisplayTile(Guid deviceId, Guid operationId)
        => await Tests.AssingDisplayTile(Factory.CreateClient(), deviceId, operationId);

    protected async Task<Guid> PostBooleanOperation(Guid deviceId, bool withEvents = true)
        => await Tests.PostBooleanOperation(Factory.CreateClient(), deviceId, withEvents);

    protected async Task<Guid> PostNumberOperation(Guid deviceId)
        => await Tests.PostNumberOperation(Factory.CreateClient(), deviceId);

    protected async Task<Guid> PostIntegerOperation(Guid deviceId)
        => await Tests.PostIntegerOperation(Factory.CreateClient(), deviceId);

    protected async Task<Guid> PostHexColorOperation(Guid deviceId)
        => await Tests.PostHexColorOperation(Factory.CreateClient(), deviceId);
    protected async Task<Guid> PostIntegerRangedOperation(Guid deviceId)
        => await Tests.PostIntegerRangedOperation(Factory.CreateClient(), deviceId);

    protected async Task<Guid> PostNumberRangedOperation(Guid deviceId)
        => await Tests.PostNumberRangedOperation(Factory.CreateClient(), deviceId);

    protected async Task<Guid> PostTextOperation(Guid deviceId)
        => await Tests.PostTextOperation(Factory.CreateClient(), deviceId);

    protected async Task<Guid> PostOptionsOperation(Guid deviceId)
        => await Tests.PostOptionsOperation(Factory.CreateClient(), deviceId);

    protected async Task<Guid> PostDashboard(Guid deviceId)
        => await Tests.PostDashboard(Factory.CreateClient(), deviceId);

    protected async Task<Guid> PostBooleanButtonWidget(Guid operationId, bool value = true)
        => await Tests.CreateButtonWidget(Factory.CreateClient(), operationId, new() { ["value"] = value });

    protected async Task<Guid> PostIntegerButtonWidget(Guid operationId, int value = 99)
        => await Tests.CreateButtonWidget(Factory.CreateClient(), operationId, new() { ["value"] = value });

    protected async Task<Guid> PostNumberButtonWidget(Guid operationId, double value = -99.123)
        => await Tests.CreateButtonWidget(Factory.CreateClient(), operationId, new() { ["value"] = value });

    protected async Task<Guid> PostTextButtonWidget(Guid operationId, string value = "#FFFFFF")
        => await Tests.CreateButtonWidget(Factory.CreateClient(), operationId, new() { ["value"] = value });

    protected async Task<Guid> PostOptionOperationWidget(Guid operationId, string value = "Option2")
        => await Tests.CreateButtonWidget(Factory.CreateClient(), operationId, new() { ["value"] = value });

    protected async Task<Guid> PostHexColorButtonWidget(Guid operationId, string value = "#06402B")
        => await Tests.CreateButtonWidget(Factory.CreateClient(), operationId, new() { ["value"] = value });

    protected async Task<Guid> PostColorWidget(Guid operationId)
        => await Tests.PostColorWidget(Factory.CreateClient(), operationId);

    protected Task<Guid> PostSliderWidget(Guid operationId)
        => Tests.CreateSliderWidget(Factory.CreateClient(), operationId);

    protected Task<Guid> PostSwitchWidget(Guid operationId)
        => Tests.CreateSwitchWidget(Factory.CreateClient(), operationId);

    protected Task<Guid> PostRadioWidget(Guid operationId)
        => Tests.CreateRadioWidget(Factory.CreateClient(), operationId);

    protected Task<Guid> PostTimeSeriesGraphWidget(Guid operationId)
        => Tests.CreateTimeSeriesGraphWidget(Factory.CreateClient(), operationId);

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
        using var scope = Factory.Services.CreateScope();
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

    #region Automations
    protected PostConditionItemRequest CreateCondition(Guid operationId, string conditionType = "BooleanCondition",
        JsonObject? conditionConfig = null)
    {
        if (conditionConfig is null)
            conditionConfig = new JsonObject { ["isOn"] = true };

        return new PostConditionItemRequest
        {
            OperationId = operationId,
            ConditionType = conditionType,
            ConditionConfiguration = conditionConfig
        };

    }

    protected PostResultItemRequest CreateResult(Guid operationId, JsonValue? valueToSend) => new()
    {
        OperationId = operationId,
        ValueToSend = valueToSend ?? JsonValue.Create(string.Empty)
    };

    protected async Task<Guid> PostAutomation(Guid homeId, IList<PostConditionItemRequest> conditions, IList<PostResultItemRequest> results)
        => await Tests.PostAutomation(Factory.CreateClient(), homeId, conditions, results);

    protected async Task<IDictionary<string, Guid>> BaseAutomationGuids()
    {
        var broker = await PostMqttBroker();
        var home = await PostAndPutBrokerForHome(broker);
        var deviceId = await PostDevice(home);
        var conditionOperation = await PostBooleanOperation(deviceId);
        var resultsOperation = await PostBooleanOperation(deviceId);

        var dashboard = await PostDashboard(deviceId);
        var widgetId = await PostBooleanButtonWidget(conditionOperation);

        return new Dictionary<string, Guid>
        {
            { "home", home }, { "broker", broker }, { "device", deviceId }, { "condition", conditionOperation },
            { "result", resultsOperation }, { "widget", widgetId }, {"dashboard", dashboard}
        };
    }


    #endregion

    #region Mqtt 
    protected async Task<IMqttClient> StartSepareateBrokerClient(string topic)
    {
        // Arrange
        var mqttClient = await StartSepareateBrokerClient();

        var topicBuilder = new MqttTopicFilterBuilder()
            .WithAtMostOnceQoS()
            .WithTopic(topic);

        // Act
        await mqttClient.SubscribeAsync(topicBuilder.Build(), default);

        return mqttClient;
    }

    protected async Task<IMqttClient> StartSepareateBrokerClient()
    {
        // Arrange
        var mqttClient = new MqttFactory().CreateMqttClient();
        var mqttOptionsBuilder = new MqttClientOptionsBuilder()
            .WithCleanSession(true)
            .WithTcpServer(MqttFixture.Hostname, MqttFixture.Port)
            .WithKeepAlivePeriod(TimeSpan.FromSeconds(120))
            .WithProtocolVersion(MqttProtocolVersion.V500)
            .WithCredentials(MqttFixture.Username, MqttFixture.Password)
            .WithClientId($"TestClient{Guid.NewGuid()}")
            .WithTlsOptions(new MqttClientTlsOptions
            {
                UseTls = MqttFixture.UseTls
            });

        // Act
        await mqttClient.ConnectAsync(mqttOptionsBuilder.Build(), default);

        // Assert
        Assert.True(mqttClient.IsConnected);

        return mqttClient;
    }

    #endregion

    #region DatabaseHelper

    protected async Task CheckIfBooleanAutomationResultSaved(Guid resultOperationId, bool expectedResult)
    {
        using var scope = Factory.Services.CreateScope();
        var operationContext = scope.ServiceProvider.GetRequiredService<DevicesContext>();
        var resultOperation = await operationContext.Operations
            .Include(o => o.Values)
            .FirstOrDefaultAsync(o => o.Id == resultOperationId);

        Assert.NotNull(resultOperation);
        var resultValue1 = resultOperation.Values.First().Value.GetValue<bool>();
        Assert.Equal(expectedResult, resultValue1);

    }

    protected async Task OperationNotTriggered(Guid resultOperationId)
    {
        using var scope = Factory.Services.CreateScope();
        var operationContext = scope.ServiceProvider.GetRequiredService<DevicesContext>();
        var resultOperation = await operationContext.Operations
            .Include(o => o.Values)
            .FirstOrDefaultAsync(o => o.Id == resultOperationId);

        Assert.NotNull(resultOperation);
        Assert.Empty(resultOperation.Values);
    }

    #endregion
}