namespace Senswave.TestInfrastructure.TestEnvironments.Common;

public class Paths
{
    #region Automations

    public const string AutomationPath = "api/v1/automations";
    public const string AutomationResultsPath = "api/v1/automations/results";
    public const string AutomationConditionsPath = "api/v1/automations/conditions";


    #endregion

    #region DataSources

    public const string BrokersPath = "api/v1/datasources/brokers";
    public static string ClientsPath(Guid brokerId) => $"api/v1/datasources/brokers/{brokerId}/clients";
    public static string SessionsPath(Guid brokerId) => $"api/v1/datasources/brokers/{brokerId}/sessions";
    public static string SubscribtionsPath(Guid brokerId) => $"api/v1/datasources/brokers/{brokerId}/subscriptions";
    public static string SubscribtionPath(Guid brokerId, Guid subscriptionId) => $"api/v1/datasources/brokers/{brokerId}/subscriptions/{subscriptionId}";

    #endregion

    #region Devices

    public const string DevicesPath = "api/v1/devices";
    public const string OperationsPath = "api/v1/devices/operations";
    public const string DashboardsPath = "api/v1/devices/dashboards";
    public const string WidgetsPath = "api/v1/devices/widgets";

    #endregion

    #region Homes

    public const string HomesPath = "api/v1/homes";
    public static string RoomsPath(Guid homeId) => $"api/v1/homes/{homeId}/rooms";
    public static string RoomPath(Guid homeId, Guid roomId) => $"api/v1/homes/{homeId}/rooms/{roomId}";
    public static string HomeSharingsPath(Guid homeId) => $"api/v1/homes/{homeId}/rooms";

    #endregion

    #region LiveUpdaes

    public const string LiveUpdatePath = "signalr/liveupdates/live";

    #endregion

    #region Users

    public const string UsersPath = "api/v1/users";

    public const string AuthPath = "api/v1/auth";
    public const string AuthPathV2 = "api/v2/auth";
    public const string LegalPath = "api/v1/legal";

    #endregion
}
