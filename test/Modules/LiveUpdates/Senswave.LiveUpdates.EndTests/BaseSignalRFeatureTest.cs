using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Senswave.Api;
using Senswave.LiveUpdates.EndTests.Extensions;
using Senswave.TestInfrastructure.TestEnvironments.Base;

namespace Senswave.LiveUpdates.EndTests;

public class BaseSignalRFeatureTest(WebApplicationFactory<Program> factory) : BaseFeatureTest(factory)
{
    public async Task<(HttpClient, HubConnection, Guid, Guid, Guid)> InitializeSignalR(bool useAdmin = false)
    {
        // Arrange
        var initialized = false;
        var brokerId = await PostBroker();
        var homeId = await PostHome();
        await PutBrokerForHome(brokerId, homeId);
        var deviceId = await PostDevice(homeId);

        var client = CreateUnauthorizedClient();
        var connection = await client.ToSignalR(CreateHandler(), useAdmin: useAdmin);
        connection.On("Initialized", () =>
        {
            initialized = true;
        });

        // Act
        await connection.StartAsync();
        await connection.InvokeAsync("Initialize", homeId.ToString());
        await Task.Delay(500);

        // Assert
        Assert.Equal(HubConnectionState.Connected, connection.State);
        Assert.True(initialized);

        return (client, connection, brokerId, homeId, deviceId);
    }
}
