using Senswave.TestInfrastructure.Extensions;
using Senswave.TestInfrastructure.TestSetup.Models;
using Senswave.TestInfrastructure.TestSetup.Models.Automations;
using Senswave.TestInfrastructure.TestSetup.Models.Common;
using Senswave.TestInfrastructure.TestSetup.Models.Devices;
using Senswave.TestInfrastructure.TestSetup.Models.Homes;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Senswave.TestInfrastructure.TestEnvironments.Common;

public class Tests
{
    #region Authorization

    public static async Task RegisterClient(HttpClient client, string email, string password)
    {
        var registerRequest = new
        {
            Email = email,
            Password = password,
            ConfirmPassword = password
        };

        //Act
        var response = await client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);

        //Assert
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, $"Failed to register reason: ({response.StatusCode}) {responseContent}");
    }

    public static async Task AuthorizeClientAsUser(HttpClient client) => await AuthorizeClientWithConsent(client);

    public static async Task AuthorizeClientWithConsent(HttpClient client, string email = "user@gmail.com", string password = "User123456!")
    {
        var loginRequest = new
        {
            Email = email,
            Password = password
        };

        //Act
        var response = await client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        //Assert
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, $"Failed to login reason: ({response.StatusCode}) {responseContent}");
        var token = JsonSerializer.Deserialize<AuthResponse>(responseContent) ?? new();

        Assert.NotEmpty(token.AccessToken);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

        await MakeConsent(client);
    }

    public static async Task AuthorizeClient(HttpClient client, string email = "user@gmail.com", string password = "User123456!")
    {
        var loginRequest = new
        {
            Email = email,
            Password = password
        };

        //Act
        var response = await client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        //Assert
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, $"Failed to login reason: ({response.StatusCode}) {responseContent}");
        var token = JsonSerializer.Deserialize<AuthResponse>(responseContent) ?? new();

        Assert.NotEmpty(token.AccessToken);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
    }

    public static Task MakeConsent(HttpClient client)
    {
        return client.PostAsync($"{Paths.UsersPath}/consents", null);
    }

    #endregion

    #region Base DataSource
    public static async Task<Guid> PostBroker(HttpClient client)
    {
        //Arrange
        Random random = new();
        var placeholder = Guid.NewGuid().ToString();

        var request = RequestFactory.CreatePostBrokerRequest(
            name: placeholder[..29],
            clientName: Guid.NewGuid().ToString(),
            url: placeholder,
            port: random.Next(5000, 10000),
            protocolVersion: "MqttV5").Serialize();

        //Act
        await AuthorizeClientAsUser(client);

        var response = await client.PostAsync(Paths.BrokersPath, request);

        //Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var postResponse = JsonSerializer.Deserialize<IdResponse>(responseContent);
        Assert.NotNull(postResponse);
        Assert.False(postResponse!.Id == Guid.Empty);

        return postResponse!.Id;
    }

    public static async Task<HttpResponseMessage> PostBrokerNoChecks(HttpClient client)
    {
        Random random = new();
        var placeholder = Guid.NewGuid().ToString();

        var request = RequestFactory.CreatePostBrokerRequest(
            name: placeholder[..29],
            clientName: Guid.NewGuid().ToString(),
            url: placeholder,
            port: random.Next(5000, 10000),
            protocolVersion: "MqttV5").Serialize();

        await AuthorizeClientAsUser(client);

        return await client.PostAsync(Paths.BrokersPath, request);
    }

    public static async Task<HttpResponseMessage> PostBrokerNoChecksPreLogged(HttpClient client)
    {
        Random random = new();
        var placeholder = Guid.NewGuid().ToString();

        var request = RequestFactory.CreatePostBrokerRequest(
            name: placeholder[..29],
            clientName: Guid.NewGuid().ToString(),
            url: placeholder,
            port: random.Next(5000, 10000),
            protocolVersion: "MqttV5").Serialize();

        return await client.PostAsync(Paths.BrokersPath, request);
    }

    public static async Task<HttpResponseMessage> DeleteBrokerNoCheks(HttpClient client, Guid brokerId)
    {
        // Act
        await AuthorizeClientAsUser(client);
        return await client.DeleteAsync($"{Paths.BrokersPath}/{brokerId}");
    }

    public static async Task<Guid> PostSubscribtion(HttpClient client, Guid brokerId)
    {
        var request = RequestFactory.CreatePostSubscribtionRequest($"test/{Guid.NewGuid()}").Serialize();
        await AuthorizeClientAsUser(client);
        var response = await client.PostAsync(Paths.SubscribtionsPath(brokerId), request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        var postResponse = JsonSerializer.Deserialize<IdResponse>(responseContent);
        Assert.NotNull(postResponse);
        return postResponse!.Id;
    }

    public static async Task<HttpResponseMessage> PostSubscribtionNoChecks(HttpClient client, Guid brokerId)
    {
        var request = RequestFactory.CreatePostSubscribtionRequest($"test/{Guid.NewGuid()}").Serialize();
        await AuthorizeClientAsUser(client);
        return await client.PostAsync(Paths.SubscribtionsPath(brokerId), request);
    }

    #endregion

    #region Base Homes

    public static async Task<Guid> PostHome(HttpClient client, int longitude, int latitude)
    {
        // Arrange
        var request = RequestFactory.CreatePostHome(
            name: Guid.NewGuid().ToString()[..29],
            icon: "default-icon",
            latitude: latitude,
            longitude: longitude).Serialize();

        // Act
        await AuthorizeClientAsUser(client);
        var response = await client.PostAsync(Paths.HomesPath, request);

        //Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var postResponse = JsonSerializer.Deserialize<IdResponse>(responseContent);
        Assert.NotNull(postResponse);
        Assert.False(postResponse!.Id == Guid.Empty);

        return postResponse!.Id;
    }

    public static async Task<HttpResponseMessage> PostHomeNoChecks(HttpClient client, int longitude, int latitude)
    {
        var request = RequestFactory.CreatePostHome(
            name: Guid.NewGuid().ToString()[..29],
            icon: "default-icon",
            latitude: latitude,
            longitude: longitude).Serialize();

        await AuthorizeClientAsUser(client);
        return await client.PostAsync(Paths.HomesPath, request);
    }

    public static async Task<Guid> PostAdminHome(HttpClient client, int longitude, int latitude)
    {
        // Arrange
        var request = RequestFactory.CreatePostHome(
            name: Guid.NewGuid().ToString()[..29],
            icon: "default-icon",
            latitude: latitude,
            longitude: longitude).Serialize();

        // Act
        await AuthorizeClientWithConsent(client, "admin@gmail.com", "Admin123456!");
        var response = await client.PostAsync(Paths.HomesPath, request);

        //Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var postResponse = JsonSerializer.Deserialize<IdResponse>(responseContent);
        Assert.NotNull(postResponse);
        Assert.False(postResponse!.Id == Guid.Empty);

        return postResponse!.Id;
    }

    public static async Task<Guid> PutBrokerForHome(HttpClient client, Guid brokerId, Guid homeId)
    {
        // Arrange
        var request = RequestFactory.CreatePutHomeBroker(brokerId: brokerId).Serialize();

        // Act
        await AuthorizeClientAsUser(client);
        var response = await client.PutAsync($"{Paths.HomesPath}/{homeId}/datasource", request);

        //Assert
        Assert.True(response.IsSuccessStatusCode, $"Failed to assign broker to home. Status code: {response.StatusCode}, Response: {await response.Content.ReadAsStringAsync()}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        return homeId;
    }

    public static async Task<Guid> PostRoom(HttpClient client, Guid homeId)
    {
        // Arrange
        var request = RequestFactory.CreatePostRoom("Testowy room" + Guid.NewGuid()).Serialize();

        // Act
        await AuthorizeClientAsUser(client);
        var response = await client.PostAsync($"{Paths.HomesPath}/{homeId}/rooms", request);

        //Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var postResponse = JsonSerializer.Deserialize<IdResponse>(responseContent);
        Assert.NotNull(postResponse);
        Assert.False(postResponse!.Id == Guid.Empty);

        return postResponse!.Id;
    }

    public static async Task<HttpResponseMessage> PostRoomNoChecks(HttpClient client, Guid homeId)
    {
        var request = RequestFactory.CreatePostRoom("Testowy pokoj" + Guid.NewGuid()).Serialize();

        await AuthorizeClientAsUser(client);
        return await client.PostAsync($"{Paths.HomesPath}/{homeId}/rooms", request);
    }

    #endregion

    #region Base Devices

    public static async Task<Guid> PostDevice(HttpClient client, Guid homeId, Guid roomId)
    {
        // Arrange
        var uniqueName = RandomExtensions.RandomString(20);

        var request = RequestFactory.CreatePostDevice(
            homeId: homeId,
            icon: "default-icon",
            name: uniqueName);

        if (roomId != Guid.Empty)
            request["roomId"] = roomId;

        // Act
        await AuthorizeClientAsUser(client);
        HttpResponseMessage response = await client.PostAsync(Paths.DevicesPath, request.Serialize());

        //Assert
        var content = await response.Content.ReadAsStringAsync();
        Assert.True(HttpStatusCode.Created == response.StatusCode, $"{content} {client.BaseAddress} {Paths.DevicesPath} {request}");

        var responseContent = await response.Content.ReadAsStringAsync();
        var postResponse = JsonSerializer.Deserialize<IdResponse>(responseContent);
        Assert.NotNull(postResponse);
        Assert.False(postResponse!.Id == Guid.Empty);

        return postResponse!.Id;
    }

    public static async Task<HttpResponseMessage> PostDeviceNoChecks(HttpClient client, Guid homeId)
    {
        var uniqueName = RandomExtensions.RandomString(20);

        var request = RequestFactory.CreatePostDevice(
            homeId: homeId,
            icon: "default-icon",
            name: uniqueName);

        await AuthorizeClientAsUser(client);
        return await client.PostAsync(Paths.DevicesPath, request.Serialize());
    }

    public static async Task AssingSwitchTile(HttpClient client, Guid deviceId, Guid operationId)
    {
        await AuthorizeClientAsUser(client);

        var request = RequestFactory.CreateAssignOperationToDevice(
            operationId: operationId.ToString(),
            type: "Switch");

        // Act
        var response = await client.PatchAsync($"{Paths.DevicesPath}/{deviceId}", request.Serialize());

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    public static async Task AssingDisplayTile(HttpClient client, Guid deviceId, Guid operationId)
    {
        await AuthorizeClientAsUser(client);

        var request = RequestFactory.CreateAssignOperationToDevice(
            displayableOperationId: operationId.ToString(),
            type: "Display");

        // Act
        var response = await client.PatchAsync($"{Paths.DevicesPath}/{deviceId}", request.Serialize());

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    public static async Task<Guid> PostDashboard(HttpClient client, Guid deviceId)
    {
        // Arrange
        var request = RequestFactory.CreatePostDashboard(
            deviceId: deviceId,
            name: Guid.NewGuid().ToString(),
            configuration: new JsonObject
            {
                ["rows"] = 6,
                ["columns"] = 4
            },
            icon: "default-icon").Serialize();

        // Act
        await AuthorizeClientAsUser(client);
        var response = await client.PostAsync(Paths.DashboardsPath, request);

        //Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var postResponse = JsonSerializer.Deserialize<IdResponse>(responseContent);
        Assert.NotNull(postResponse);
        Assert.False(postResponse!.Id == Guid.Empty);

        return postResponse!.Id;
    }

    public static async Task<HttpResponseMessage> PostDashboardNoChecks(HttpClient client, Guid deviceId)
    {
        // Arrange
        var request = RequestFactory.CreatePostDashboard(
            deviceId: deviceId,
            name: Guid.NewGuid().ToString(),
            configuration: new JsonObject
            {
                ["rows"] = 6,
                ["columns"] = 4
            },
            icon: "default-icon").Serialize();

        // Act
        await AuthorizeClientAsUser(client);
        return await client.PostAsync(Paths.DashboardsPath, request);
    }

    public static async Task SetWidgetOnDashboard(HttpClient client, Guid dashboardId, Guid widgetId)
    {
        // Arrange
        var request = new JsonObject()
        {
            ["widgetId"] = widgetId,
            ["row"] = 0,
            ["rowSpan"] = 1,
            ["column"] = 0,
            ["columnSpan"] = 2
        };

        // Act
        await AuthorizeClientAsUser(client);
        var response = await client.PutAsync($"{Paths.DashboardsPath}/{dashboardId}/widgets", request.Serialize());

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    #region Operations
    public static async Task<Guid> PostBooleanOperation(HttpClient client, Guid deviceId, bool withEvents, string? topic = null)
    {
        // Arrange
        topic ??= Guid.NewGuid().ToString().Replace("-", "/");
        var request = RequestFactory.CreatePostOperation(
            deviceId: deviceId,
            name: Guid.NewGuid().ToString()[..29],
            type: "Boolean",
            configuration: new JsonObject
            {
                ["jsonNames"] = new JsonArray("value"),
                ["isJson"] = true,
                ["saveOnUserAction"] = withEvents
            },
            topic: topic);

        // Act
        await AuthorizeClientAsUser(client);
        var response = await client.PostAsync(Paths.OperationsPath, request.Serialize());

        //Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var postResponse = JsonSerializer.Deserialize<IdResponse>(responseContent);
        Assert.NotNull(postResponse);
        Assert.False(postResponse!.Id == Guid.Empty);

        return postResponse!.Id;
    }

    public static async Task<Guid> PostHexColorOperation(HttpClient client, Guid deviceId, JsonObject? configuration = null)
    {
        configuration ??= new JsonObject
        {
            ["jsonNames"] = new JsonArray("newValue"),
            ["isJson"] = true
        };

        // Arrange
        var request = RequestFactory.CreatePostOperation(
            deviceId: deviceId,
            name: Guid.NewGuid().ToString()[..29],
            type: "HexColor",
            configuration: configuration,
            topic: Guid.NewGuid().ToString()).Serialize();

        // Act
        await AuthorizeClientAsUser(client);
        var response = await client.PostAsync(Paths.OperationsPath, request);

        //Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var postResponse = JsonSerializer.Deserialize<IdResponse>(responseContent);
        Assert.NotNull(postResponse);
        Assert.False(postResponse!.Id == Guid.Empty);

        return postResponse!.Id;
    }

    public static async Task<Guid> PostIntegerRangedOperation(HttpClient client, Guid deviceId)
    {
        // Arrange
        var request = RequestFactory.CreatePostOperation(
            deviceId: deviceId,
            name: Guid.NewGuid().ToString()[..29],
            type: "Integer",
            configuration: new JsonObject
            {
                ["min"] = -100,
                ["max"] = 100,
                ["jsonNames"] = new JsonArray("value"),
                ["isJson"] = true
            },
            topic: Guid.NewGuid().ToString()).Serialize();

        // Act
        await AuthorizeClientAsUser(client);
        var response = await client.PostAsync(Paths.OperationsPath, request);

        //Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var postResponse = JsonSerializer.Deserialize<IdResponse>(responseContent);
        Assert.NotNull(postResponse);
        Assert.False(postResponse!.Id == Guid.Empty);

        return postResponse!.Id;
    }

    public static async Task<Guid> PostIntegerOperation(HttpClient client, Guid deviceId)
    {
        // Arrange
        var request = RequestFactory.CreatePostOperation(
            deviceId: deviceId,
            name: Guid.NewGuid().ToString()[..29],
            type: "Integer",
            configuration: new JsonObject
            {
                ["jsonNames"] = new JsonArray("value"),
                ["isJson"] = true
            },
            topic: Guid.NewGuid().ToString()).Serialize();

        // Act
        await AuthorizeClientAsUser(client);
        var response = await client.PostAsync(Paths.OperationsPath, request);

        //Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var postResponse = JsonSerializer.Deserialize<IdResponse>(responseContent);
        Assert.NotNull(postResponse);
        Assert.False(postResponse!.Id == Guid.Empty);

        return postResponse!.Id;
    }

    public static async Task<Guid> PostNumberRangedOperation(HttpClient client, Guid deviceId)
    {
        // Arrange
        var request = RequestFactory.CreatePostOperation(
            deviceId: deviceId,
            name: Guid.NewGuid().ToString()[..29],
            type: "Number",
            configuration: new JsonObject
            {
                ["min"] = -100.1,
                ["max"] = 100.1,
                ["jsonNames"] = new JsonArray("value"),
                ["isJson"] = true
            },
            topic: Guid.NewGuid().ToString()).Serialize();

        // Act
        await AuthorizeClientAsUser(client);
        var response = await client.PostAsync(Paths.OperationsPath, request);

        //Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var postResponse = JsonSerializer.Deserialize<IdResponse>(responseContent);
        Assert.NotNull(postResponse);
        Assert.False(postResponse!.Id == Guid.Empty);

        return postResponse!.Id;
    }

    public static async Task<Guid> PostNumberOperation(HttpClient client, Guid deviceId)
    {
        // Arrange
        var request = RequestFactory.CreatePostOperation(
            deviceId: deviceId,
            name: Guid.NewGuid().ToString()[..29],
            type: "Number",
            configuration: new JsonObject
            {
                ["jsonNames"] = new JsonArray("value"),
                ["isJson"] = true
            },
            topic: Guid.NewGuid().ToString()).Serialize();

        // Act
        await AuthorizeClientAsUser(client);
        var response = await client.PostAsync(Paths.OperationsPath, request);

        //Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var postResponse = JsonSerializer.Deserialize<IdResponse>(responseContent);
        Assert.NotNull(postResponse);
        Assert.False(postResponse!.Id == Guid.Empty);

        return postResponse!.Id;
    }

    public static async Task<Guid> PostTextOperation(HttpClient client, Guid deviceId)
    {
        // Arrange
        var request = RequestFactory.CreatePostOperation(
            deviceId: deviceId,
            name: Guid.NewGuid().ToString()[..29],
            type: "Text",
            configuration: new JsonObject
            {
                ["jsonNames"] = new JsonArray("value"),
                ["isJson"] = true
            },
            topic: Guid.NewGuid().ToString()).Serialize();

        // Act
        await AuthorizeClientAsUser(client);
        var response = await client.PostAsync(Paths.OperationsPath, request);

        //Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var postResponse = JsonSerializer.Deserialize<IdResponse>(responseContent);
        Assert.NotNull(postResponse);
        Assert.False(postResponse!.Id == Guid.Empty);

        return postResponse!.Id;
    }

    public static async Task<Guid> PostOptionsOperation(HttpClient client, Guid deviceId, JsonArray? options = null)
    {
        var optionsList = options ??
        [
            new JsonObject
            {
                ["name"] = "Option1",
                ["value"] = "Value1"
            },
            new JsonObject
            {
                ["name"] = "Option2",
                ["value"] = 12.31
            },
            new JsonObject
            {
                ["name"] = "Option3",
                ["value"] = 1
            },
            new JsonObject
            {
                ["name"] = "Option4",
                ["value"] = true
            }
        ];

        // Arrange
        var request = RequestFactory.CreatePostOperation(
            deviceId: deviceId,
            name: Guid.NewGuid().ToString()[..29],
            type: "Options",
            configuration: new JsonObject
            {
                ["jsonNames"] = new JsonArray("value"),
                ["isJson"] = true,
                ["options"] = optionsList
            },
            topic: Guid.NewGuid().ToString()).Serialize();

        // Act
        await AuthorizeClientAsUser(client);
        var response = await client.PostAsync(Paths.OperationsPath, request);

        //Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var postResponse = JsonSerializer.Deserialize<IdResponse>(responseContent);
        Assert.NotNull(postResponse);
        Assert.False(postResponse!.Id == Guid.Empty);

        return postResponse!.Id;
    }

    public static async Task<HttpResponseMessage> PostBooleanOperationNoChecks(HttpClient client, Guid deviceId)
    {
        // Arrange
        var request = RequestFactory.CreatePostOperation(
            deviceId: deviceId,
            name: Guid.NewGuid().ToString()[..29],
            type: "Boolean",
            configuration: new JsonObject
            {
                ["jsonNames"] = new JsonArray("value"),
                ["isJson"] = true,
                ["treatAsString"] = true
            },
            topic: Guid.NewGuid().ToString()).Serialize();

        // Act
        await AuthorizeClientAsUser(client);
        return await client.PostAsync(Paths.OperationsPath, request);
    }

    #endregion

    #region Widgets

    public static async Task<Guid> CreateButtonWidget(HttpClient client, Guid operationId, JsonObject config)
    {
        // Arrange
        var request = RequestFactory.CreatePostWidget(
            operationId: operationId,
            name: "Widget" + Guid.NewGuid(),
            type: "Button",
            config: config);

        // Act
        await AuthorizeClientAsUser(client);
        var response = await client.PostAsync(Paths.WidgetsPath, request.Serialize());

        //Assert
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var postResponse = JsonSerializer.Deserialize<IdResponse>(responseContent);
        Assert.NotNull(postResponse);
        Assert.False(postResponse!.Id == Guid.Empty);

        return postResponse!.Id;
    }

    public static async Task<Guid> PostColorWidget(HttpClient client, Guid operationId, JsonObject? config = null)
    {
        config ??= [];

        // Arrange
        var request = RequestFactory.CreatePostWidget(
            operationId: operationId,
            name: "Widget" + Guid.NewGuid(),
            type: "Color",
            config: config);

        // Act
        await AuthorizeClientAsUser(client);
        var response = await client.PostAsync(Paths.WidgetsPath, request.Serialize());

        //Assert
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var postResponse = JsonSerializer.Deserialize<IdResponse>(responseContent);
        Assert.NotNull(postResponse);
        Assert.False(postResponse!.Id == Guid.Empty);

        return postResponse!.Id;
    }

    public static async Task<Guid> CreateSliderWidget(HttpClient client, Guid operationId, JsonObject? config = null)
    {
        config ??= new JsonObject
        {
            ["step"] = 1
        };

        // Arrange
        var request = RequestFactory.CreatePostWidget(
            operationId: operationId,
            name: "Widget" + Guid.NewGuid(),
            type: "Slider",
            config: config);

        // Act
        await AuthorizeClientAsUser(client);
        var response = await client.PostAsync(Paths.WidgetsPath, request.Serialize());

        //Assert
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var postResponse = JsonSerializer.Deserialize<IdResponse>(responseContent);
        Assert.NotNull(postResponse);
        Assert.False(postResponse!.Id == Guid.Empty);

        return postResponse!.Id;
    }

    public static async Task<Guid> CreateSwitchWidget(HttpClient client, Guid operationId, JsonObject? config = null)
    {
        config ??= [];

        // Arrange
        var request = RequestFactory.CreatePostWidget(
            operationId: operationId,
            name: "Widget" + Guid.NewGuid(),
            type: "Switch",
            config: config);

        // Act
        await AuthorizeClientAsUser(client);
        var response = await client.PostAsync(Paths.WidgetsPath, request.Serialize());

        //Assert
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var postResponse = JsonSerializer.Deserialize<IdResponse>(responseContent);
        Assert.NotNull(postResponse);
        Assert.False(postResponse!.Id == Guid.Empty);

        return postResponse!.Id;
    }

    public static async Task<Guid> CreateTimeSeriesGraphWidget(HttpClient client, Guid operationId, JsonObject? config = null)
    {
        config ??= new JsonObject
        {
            ["displayUnit"] = "%",
            ["initialNumberOfData"] = 100,
            ["displayType"] = "lines"
        };

        var request = RequestFactory.CreatePostWidget(
            operationId: operationId,
            name: "Widget" + Guid.NewGuid(),
            type: "TimeSeriesGraph",
            config: config);

        await AuthorizeClientAsUser(client);
        var response = await client.PostAsync(Paths.WidgetsPath, request.Serialize());

        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var postResponse = JsonSerializer.Deserialize<IdResponse>(responseContent);
        Assert.NotNull(postResponse);
        Assert.False(postResponse!.Id == Guid.Empty);

        return postResponse!.Id;
    }

    public static async Task<Guid> CreateRadioWidget(HttpClient client, Guid operationId, JsonObject? config = null)
    {
        config ??= new JsonObject
        {
            ["options"] = new JsonArray(
                new JsonObject { ["displayName"] = "Option 1", ["icon"] = "Icon1", ["optionName"] = "Option1" },
                new JsonObject { ["displayName"] = "Option 2", ["icon"] = "Icon1", ["optionName"] = "Option2" },
                new JsonObject { ["displayName"] = "Option 3", ["icon"] = "Icon31", ["optionName"] = "Option3" }
            )
        };

        // Arrange

        var request = RequestFactory.CreatePostWidget(
            operationId: operationId,
            name: "Widget" + Guid.NewGuid(),
            type: "Radio",
            config: config);

        // Act

        await AuthorizeClientAsUser(client);
        var response = await client.PostAsync(Paths.WidgetsPath, request.Serialize());

        //Assert
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var postResponse = JsonSerializer.Deserialize<IdResponse>(responseContent);
        Assert.NotNull(postResponse);
        Assert.False(postResponse!.Id == Guid.Empty);
        return postResponse!.Id;
    }

    #endregion

    #endregion

    #region Sharing

    public static async Task<InviteFriendResponse> InviteFriends(HttpClient client, JsonObject request)
    {
        // Act
        await AuthorizeClientAsUser(client);
        var response = await client.PostAsync($"{Paths.HomesPath}/sharings", request.Serialize());

        var responseDto = JsonSerializer.Deserialize<InviteFriendResponse>(await response.Content.ReadAsStringAsync());

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(responseDto);
        Assert.NotNull(responseDto.Password);
        Assert.False(responseDto.InvitationId == Guid.Empty);

        return responseDto;
    }

    public static async Task AcceptInvitation(HttpClient acceptor, string password)
    {
        // Arrange
        var request = RequestFactory.CreateAcceptInvitation(password);

        // Act
        var response = await acceptor.PutAsync($"{Paths.HomesPath}/sharings", request.Serialize());

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    public static async Task<HttpResponseMessage> AcceptInvitationNoCheck(HttpClient acceptor, string password)
    {
        var request = RequestFactory.CreateAcceptInvitation(password);

        return await acceptor.PutAsync($"{Paths.HomesPath}/sharings", request.Serialize());
    }

    public static async Task PrepareDeviceSharings(HttpClient client, string email, string sharingType, Guid deviceId)
    {
        // Arrange
        var request = RequestFactory.CreatePostDeviceSharing(
            deviceId: deviceId,
            friendEmail: email,
            sharingType: sharingType);

        // Act
        await AuthorizeClientAsUser(client);
        var response = await client.PutAsync($"{Paths.DevicesPath}/sharings", request.Serialize());

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    public static async Task<IList<Guid>> GetDeviceSharings(HttpClient client, Guid device)
    {
        // Act
        await AuthorizeClientAsUser(client);
        var getResponse = await client.GetAsync($"{Paths.DevicesPath}/sharings?deviceId={device}");
        var content = await getResponse.Content.ReadAsStringAsync();
        var getResponseDto = JsonSerializer.Deserialize<GetDeviceTestResponse>(content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(getResponseDto);

        return getResponseDto.Items
            .Where(x => x.SharingId is not null)
            .Select(x => x.SharingId!.Value)
            .ToList();
    }
    #endregion

    #region Automations

    public static async Task<Guid> PostAutomation(HttpClient client, Guid homeId, IList<PostConditionItemRequest> conditions,
        IList<PostResultItemRequest> results)
    {
        await AuthorizeClientAsUser(client);

        var request = new CreateAutomationRequest
        {
            HomeId = homeId,
            Name = Guid.NewGuid().ToString()[1..30],
            Icon = "gear",
            ConditionConnector = "And",
            Conditions = conditions,
            Results = results
        };

        // Act
        var response = await client.PostAsync(Paths.AutomationPath, request.Serialize());
        var responseDto = JsonSerializer.Deserialize<IdResponse>(await response.Content.ReadAsStringAsync());

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(responseDto);

        return responseDto.Id;
    }

    public static async Task<HttpResponseMessage> PostAutomationNoChecks(
        HttpClient client,
        Guid homeId,
        IList<PostConditionItemRequest> conditions,
        IList<PostResultItemRequest> results)
    {
        await AuthorizeClientAsUser(client);

        var request = new CreateAutomationRequest
        {
            HomeId = homeId,
            Name = Guid.NewGuid().ToString()[1..29],
            Icon = "gear",
            ConditionConnector = "And",
            Conditions = conditions,
            Results = results
        };

        return await client.PostAsync(Paths.AutomationPath, request.Serialize());
    }

    #endregion
}
