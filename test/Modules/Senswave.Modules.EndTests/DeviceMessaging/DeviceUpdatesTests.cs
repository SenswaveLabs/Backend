using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using Senswave.DataSources.BrokerConnection.Interfaces;
using Senswave.DataSources.Infrastructure;
using Senswave.Devices.Infrastructure;
using Senswave.LiveUpdates.EndTests.Extensions;
using Senswave.TestInfrastructure.TestEnvironments.Mqtt;
using System.Text.Json.Nodes;

namespace Senswave.Modules.EndTests.DeviceMessaging;

[Collection("Mqtt E2E Tests Collection")]
[Trait("Collection", "MqttEndTest")]
public class DeviceUpdatesTests(MqttTestEnvironment factory) : MqttFeatureTest(factory)
{
    [Fact]
    public async Task DeviceUpdateSendsInformationToUsers()
    {
        // Arrange
        var client = await CreateUser();
        var connection = await client.ToSignalR(CreateHandler());

        var broker = await PostMqttBroker();
        var home = await PostAndPutBrokerForHome(broker);
        var device = await PostDevice(home);
        var booleanOperation = await PostBooleanOperation(device);
        var widget = await PostBooleanButtonWidget(booleanOperation);
        await PatchDeviceWithSwitchTile(device, booleanOperation);

        var initialized = false;
        connection.On("Initialized", () =>
        {
            initialized = true;
        });

        var updates = new List<(string, JsonObject)>();
        connection.On<string, JsonObject>("Update", (actionName, data) =>
        {
            updates.Add((actionName, data));
        });

        var clients = Services.GetRequiredService<IClientService>();
        var brokerConnection = clients.GetClient(broker);

        using var scope = Factory.Server.Services.CreateScope();

        var devicesContext = scope.ServiceProvider.GetService<DevicesContext>()!;
        var dataSourcesContext = scope.ServiceProvider.GetService<DataSourcesContext>()!;

        var brokerClient = await StartSepareateBrokerClient();

        // Act
        await connection.StartAsync();
        await connection.InvokeAsync("Initialize", home.ToString());

        await StartBrokerClient(broker);

        var operation = await devicesContext.Operations
            .Include(x => x.DataReference)
            .FirstAsync(x => x.Id == booleanOperation);

        var subscribtion = await dataSourcesContext.Subscribtions
            .FirstAsync(x => x.Id == operation.DataReference!.DataSourceDataReferenceId);

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(subscribtion.Topic)
            .WithPayload("{\"value\":true}")
            .Build();

        await brokerClient.PublishAsync(message, default);

        await Task.Delay(500);

        // Assert
        Assert.Equal(HubConnectionState.Connected, connection.State);
        Assert.True(initialized);
        Assert.Equal(3, updates.Count);
        Assert.Contains(updates, x => x.Item1 == "widgetsActionUpdate");
        Assert.Single(updates.Where(x => x.Item1 == "widgetsActionUpdate").First().Item2["items"]!.AsArray());
        Assert.Contains(updates, x => x.Item1 == "deviceTileActionUpdate");
        Assert.Contains(updates, x => x.Item1 == "dataSourceStateUpdate");

        // Cleanup
        await MqttHelpers.Cleanup(brokerClient);
    }
}
