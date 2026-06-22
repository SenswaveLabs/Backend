using System.Text.Json.Nodes;

namespace Senswave.TestInfrastructure.TestSetup.Models;

public static class RequestFactory
{
    #region Automations

    // TODO: For now it is empty, fill with proper creators after refactoring in Automation module

    #endregion

    #region DataSources

    public static JsonObject CreatePatchBrokerRequest(
        string name = "",
        string url = "",
        string clientName = "",
        int port = 0,
        bool? useTls = null,
        string protocolVersion = "",
        string username = "",
        string password = ""
    ) => CreateBrokerRequest(name, url, clientName, port, useTls, protocolVersion, username, password);


    public static JsonObject CreatePostBrokerRequest(
        string name = "",
        string url = "",
        string clientName = "",
        int port = 0,
        bool? useTls = false,
        string protocolVersion = "",
        string username = "",
        string password = ""
    ) => CreateBrokerRequest(name, url, clientName, port, useTls, protocolVersion, username, password);

    public static JsonObject CreateStartClient(string username = "", string password = "")
        => new()
        {
            ["username"] = username,
            ["password"] = password
        };

    public static JsonObject CreatePostSubscribtionRequest(string topic = "")
        => new()
        {
            ["topic"] = topic
        };

    private static JsonObject CreateBrokerRequest(
        string name = "",
        string url = "",
        string clientName = "",
        int port = 0,
        bool? useTls = null,
        string protocolVersion = "",
        string username = "",
        string password = ""
    ) => new()
    {
        ["name"] = name,
        ["url"] = url,
        ["clientName"] = clientName,
        ["port"] = port,
        ["protocolVersion"] = protocolVersion,
        ["useTls"] = useTls,
        ["password"] = password,
        ["username"] = username
    };

    #endregion

    #region Devices

    public static JsonObject CreateActionRequest(JsonNode? value) => new()
    {
        ["value"] = value
    };

    public static JsonObject CreatePatchDashboard(Guid deviceId, string name = "", string icon = "")
        => CreateDashboard(deviceId, name, icon);

    public static JsonObject CreatePostDashboard(Guid deviceId = default, string name = "", string icon = "",
        JsonObject? configuration = null)
        => CreateDashboard(deviceId, name, icon, configuration);

    public static JsonObject CreatePatchDevice(Guid? roomId = default, string name = "", string icon = "",
        Guid operationId = default, string type = "",
        Guid presenceOperationId = default, string presenceType = "")
        => CreateDevice(roomId, name, icon, operationId, type, presenceOperationId: presenceOperationId, presenceType: presenceType);

    public static JsonObject CreatePostDevice(Guid roomId = default, string name = "",
        string icon = "", Guid operationId = default, string type = "", Guid homeId = default)
        => CreateDevice(roomId, name, icon, operationId, type, homeId);


    public static JsonObject CreatePostDeviceSharing(Guid deviceId = default, string sharingType = "",
        string friendEmail = "")
        => new()
        {
            ["deviceId"] = deviceId,
            ["sharingType"] = sharingType,
            ["friendEmail"] = friendEmail
        };

    public static JsonObject CreatePostOperation(Guid deviceId = default, string name = "", string type = "",
        JsonObject? configuration = null, string topic = "")
        => new()
        {
            ["deviceId"] = deviceId,
            ["name"] = name,
            ["type"] = type,
            ["configuration"] = configuration,
            ["topic"] = topic
        };

    public static JsonObject CreatePostWidget(Guid operationId = default, string name = "",
        string type = "", JsonObject? config = null) => new()
        {
            ["operationId"] = operationId,
            ["name"] = name,
            ["type"] = type,
            ["configuration"] = config
        };

    public static JsonObject CreateAssignOperationToDevice(string operationId = "", string type = "",
        string displayableOperationId = "", JsonObject? configuration = null)
        => new()
        {
            ["operationId"] = string.IsNullOrEmpty(operationId) ? null : (JsonNode)operationId,
            ["displayableOperationId"] = string.IsNullOrEmpty(displayableOperationId) ? null : (JsonNode)displayableOperationId,
            ["configuration"] = configuration,
            ["type"] = type
        };


    private static JsonObject CreateDevice(Guid? roomId = default, string name = "",
        string icon = "", Guid operationId = default, string type = "", Guid homeId = default,
        Guid presenceOperationId = default, string presenceType = "")
        => new()
        {
            ["homeId"] = homeId,
            ["roomId"] = roomId,
            ["name"] = name,
            ["icon"] = icon,
            ["operationId"] = operationId,
            ["type"] = type,
            ["presenceOperationId"] = presenceOperationId,
            ["presenceType"] = presenceType
        };

    private static JsonObject CreateDashboard(Guid deviceId = default, string name = "", string icon = "",
        JsonObject? configuration = null)
        => new()
        {
            ["deviceId"] = deviceId,
            ["name"] = name,
            ["icon"] = icon,
            ["configuration"] = configuration
        };

    #endregion

    #region Homes

    public static JsonObject CreatePatchHome(string name = "", string icon = "", double latitude = 0.0,
        double longitude = 0.0)
        => CreateHome(name, icon, latitude, longitude);

    public static JsonObject CreatePostHome(string name = "", string icon = "", double latitude = 0.0,
        double longitude = 0.0)
        => CreateHome(name, icon, latitude, longitude);

    public static JsonObject CreatePatchRoom(string name = "")
        => ApplyNameAttribute(name);

    public static JsonObject CreatePostRoom(string name = "")
        => ApplyNameAttribute(name);

    public static JsonObject CreatePutHomeBroker(Guid brokerId = default)
        => new()
        {
            ["brokerId"] = brokerId
        };

    public static JsonObject CreateAcceptInvitation(string? password = "")
        => new()
        {
            ["password"] = password
        };

    public static JsonObject CreateFriendInvitation(Guid homeId = default, string friendEmail = "",
        string sharingType = "")
        => new()
        {
            ["homeId"] = homeId,
            ["friendEmail"] = friendEmail,
            ["sharingType"] = sharingType
        };

    private static JsonObject ApplyNameAttribute(string name = "")
        => new()
        {
            ["name"] = name
        };

    private static JsonObject CreateHome(string name = "", string icon = "", double latitude = 0.0,
        double longitude = 0.0)
        => new()
        {
            ["name"] = name,
            ["icon"] = icon,
            ["latitude"] = latitude,
            ["longitude"] = longitude
        };

    #endregion

}