namespace Senswave.Homes.Application.Homes.Features.DeleteHomeDataSource;

internal class DeleteHomeDataSourceErrors
{
    public static Error HomeNotFound = Error.NotFound("HomeNotFound", "Home not found for user.");

    public static Error DevicesExist = Error.Conflict("DevicesExist", "Cannot change data source because there are devices in the home.");

    public static Error FailedToCleanSubscribtionsForDataSource = Error.Conflict("FailedToCleanSubscribtionsForDataSource", "Failed to clean subscriptions for the data source.");
}
