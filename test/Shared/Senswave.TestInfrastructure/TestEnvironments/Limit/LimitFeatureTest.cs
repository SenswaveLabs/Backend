using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Senswave.Api;
using Senswave.Automations.Infrastructure;
using Senswave.DataSources.Infrastructure;
using Senswave.Devices.Infrastructure;
using Senswave.Homes.Infrastructure;
using Senswave.TestInfrastructure.TestEnvironments.Common;
using Senswave.TestInfrastructure.TestSetup.Models;
using Senswave.TestInfrastructure.TestSetup.Models.Automations;
using Senswave.TestInfrastructure.TestSetup.Models.Homes;
using System.Text.Json.Nodes;

namespace Senswave.TestInfrastructure.TestEnvironments.Limit;

public class LimitFeatureTest(WebApplicationFactory<Program> factory) : IClassFixture<LimitTestEnvironment>
{
    #region Privileges

    public static readonly List<object[]> HomePriviliges =
    [
        ["Manage"],
        ["Display"]
    ];

    public static readonly List<object[]> DevicePrivileges =
    [
        ["Display"],
        ["Action"],
        ["Manage"]
    ];

    public static IEnumerable<object[]> NotManageDevicePrivileges =>
    [
        [_displayPrivilege],
        [_actionPrivilege],
    ];

    public static IEnumerable<object[]> GetDevicePrivileges => DevicePrivileges.GetRange(0, 3);
    public static IEnumerable<object[]> InvokeDevicePrivileges => DevicePrivileges.GetRange(1, 2);
    public static IEnumerable<object[]> ManageDevicePrivileges => DevicePrivileges.GetRange(2, 1);

    protected const string _managePrivilege = "Manage";
    protected const string _displayPrivilege = "Display";
    protected const string _actionPrivilege = "Action";

    #endregion

    protected WebApplicationFactory<Program> Factory => factory;

    protected HttpClient CreateClient() => Factory.CreateClient();

    #region Authorization

    protected static async Task AuthorizeClientAsUser(HttpClient client)
        => await Tests.AuthorizeClientAsUser(client);

    protected static async Task AuthorizeClientAsUser1(HttpClient client)
        => await Tests.AuthorizeClientWithConsent(client, "user1@gmail.com", "User123456!");

    protected static async Task AuthorizeClientAsAdmin(HttpClient client)
        => await Tests.AuthorizeClientWithConsent(client, "admin@gmail.com", "Admin123456!");

    #endregion

    #region DataSource

    protected async Task<Guid> PostBroker()
        => await Tests.PostBroker(Factory.CreateClient());

    protected async Task<HttpResponseMessage> PostBrokerNoChecks()
        => await Tests.PostBrokerNoChecks(Factory.CreateClient());

    protected async Task<HttpResponseMessage> PostBrokerNoChecksPreLogged(HttpClient client)
        => await Tests.PostBrokerNoChecksPreLogged(client);

    protected async Task<Guid> PostSubscribtion(Guid brokerId)
        => await Tests.PostSubscribtion(Factory.CreateClient(), brokerId);

    protected async Task<HttpResponseMessage> PostSubscribtionNoChecks(Guid brokerId)
        => await Tests.PostSubscribtionNoChecks(Factory.CreateClient(), brokerId);

    #endregion

    #region Homes

    protected async Task<Guid> PostHome(int longitude = 76, int latitude = 51)
        => await Tests.PostHome(Factory.CreateClient(), longitude, latitude);

    protected async Task<HttpResponseMessage> PostHomeNoChecks(int longitude = 76, int latitude = 51)
        => await Tests.PostHomeNoChecks(Factory.CreateClient(), longitude, latitude);

    protected async Task<Guid> PostHomeWithBroker()
    {
        var home = await PostHome(75, 51);
        var broker = await PostBroker();
        return await Tests.PutBrokerForHome(Factory.CreateClient(), broker, home);
    }

    protected async Task<Guid> PostRoom(Guid homeId)
        => await Tests.PostRoom(Factory.CreateClient(), homeId);

    protected async Task<HttpResponseMessage> PostRoomNoChecks(Guid homeId)
        => await Tests.PostRoomNoChecks(Factory.CreateClient(), homeId);

    #endregion

    #region Devices

    protected async Task<Guid> PostDevice(Guid homeId, Guid roomId = default)
        => await Tests.PostDevice(Factory.CreateClient(), homeId, roomId);

    protected async Task<HttpResponseMessage> PostDeviceNoChecks(Guid homeId)
        => await Tests.PostDeviceNoChecks(Factory.CreateClient(), homeId);

    protected async Task<Guid> PostBooleanOperation(Guid deviceId, string? topic= null)
        => await Tests.PostBooleanOperation(Factory.CreateClient(), deviceId, true, topic);

    protected async Task<HttpResponseMessage> PostBooleanOperationNoChecks(Guid deviceId)
        => await Tests.PostBooleanOperationNoChecks(Factory.CreateClient(), deviceId);

    protected async Task<Guid> PostDashboard(Guid deviceId)
        => await Tests.PostDashboard(Factory.CreateClient(), deviceId);

    protected async Task<HttpResponseMessage> PostDashboardNoChecks(Guid deviceId)
        => await Tests.PostDashboardNoChecks(Factory.CreateClient(), deviceId);

    #endregion

    #region Sharing

    protected async Task<InviteFriendResponse> InviteFriend(Guid homeId, string friendEmail = "admin@gmail.com",
        string sharingType = "Manage")
        => await Tests.InviteFriends(Factory.CreateClient(), RequestFactory.CreateFriendInvitation(
            homeId: homeId, sharingType: sharingType, friendEmail: friendEmail));

    protected static async Task AcceptInvitation(HttpClient acceptor, string password)
        => await Tests.AcceptInvitation(acceptor, password);

    protected static async Task<HttpResponseMessage> AcceptInvitationNoCheck(HttpClient acceptor, string password)
        => await Tests.AcceptInvitationNoCheck(acceptor, password);

    protected async Task PrepareHomeSharingForAdmin(Guid homeId, string sharingType = "Manage")
    {
        var passwordResponse = await InviteFriend(homeId, "admin@gmail.com", sharingType);
        var friend = CreateClient();
        await AuthorizeClientAsAdmin(friend);
        await AcceptInvitation(friend, passwordResponse.Password);
    }

    #endregion

    #region Automations

    protected async Task PostAutomation(Guid home)
    {
        var device = await PostDevice(home);
        var conditionOperation = await PostBooleanOperation(device);
        var resultOperation = await PostBooleanOperation(device);

        var conditions = new List<PostConditionItemRequest>()
        {
            CreateCondition(conditionOperation)
        };

        var results = new List<PostResultItemRequest>()
        {
            CreateResult(resultOperation)
        };

        await Tests.PostAutomation(Factory.CreateClient(), home, conditions, results);
    }

    protected async Task<HttpResponseMessage> PostAutomationNoChecks(Guid home)
    {
        var device = await PostDevice(home);
        var conditionOperation = await PostBooleanOperation(device);
        var resultOperation = await PostBooleanOperation(device);

        var conditions = new List<PostConditionItemRequest>()
        {
            CreateCondition(conditionOperation)
        };

        var results = new List<PostResultItemRequest>()
        {
            CreateResult(resultOperation)
        };

        return await Tests.PostAutomationNoChecks(Factory.CreateClient(), home, conditions, results);
    }

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

    protected PostResultItemRequest CreateResult(Guid operationId, JsonValue? valueToSend = null) => new()
    {
        OperationId = operationId,
        ValueToSend = valueToSend ?? JsonValue.Create(string.Empty)
    };

    #endregion

    #region Cleanup

    public async Task CleanBrokers()
    {
        using var scope = Factory.Server.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<DataSourcesContext>();
        var brokers = await context.Brokers
            .ToListAsync();

        context.Brokers.RemoveRange(brokers);
        await context.SaveChangesAsync();
    }

    public async Task CleanHomes()
    {
        using var scope = Factory.Server.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<HomesContext>();
        var homes = await context.Homes
            .Include(x => x.Rooms)
            .Include(x => x.HomeSharing)
            .ToListAsync();

        context.Homes.RemoveRange(homes);

        var invitation = await context.HomeInvitations.ToListAsync();

        context.HomeInvitations.RemoveRange(invitation);
        await context.SaveChangesAsync();

        await CleanBrokers();
    }

    public async Task CleanDevices()
    {
        using var scope = Factory.Server.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<DevicesContext>();

        var devices = await context.Devices
            .Include(x => x.Operations)
            .Include(x => x.DeviceSharings)
            .Include(x => x.DataReferences)
            .Include(x => x.Dashboards)
            .ToListAsync();

        foreach (var device in devices)
        {
            context.Operations.RemoveRange(device.Operations);
            context.DeviceSharings.RemoveRange(device.DeviceSharings);
            context.DataReferences.RemoveRange(device.DataReferences);

            if (device.Dashboards != null)
            {
                context.Dashboards.RemoveRange(device.Dashboards);
            }
        }

        context.Devices.RemoveRange(devices);

        await context.SaveChangesAsync();

        await CleanHomes();
    }

    public async Task CleanAutomations()
    {
        using var scope = Factory.Server.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AutomationsContext>();

        var automations = await context.Automations
            .Include(x => x.Results)
            .Include(x => x.Conditions)
            .ToListAsync();

        context.RemoveRange(automations);
        await context.SaveChangesAsync();

        await CleanDevices();
    }

    #endregion
}