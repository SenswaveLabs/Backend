namespace Senswave.Homes.Application.Homes.Features.AssignHomeDataSource;

internal class AssignHomeDataSourceErrors
{
    public static Error HomeNotFound =
        Error.NotFound("HomeNotFound", "Home not found for user.");

    public static Error DataSourceIsNotOwned =
        Error.Failure("DataSourceIsNotOwned", "User is not the owner of the data source and cannot perform this action.");

    public static Error DevicesExist =
        Error.Conflict("DevicesExist", "Cannot change data source because there are devices in the home.");

    public static Error DataSourceAlreadyUsed =
        Error.Conflict("DataSourceAlreadyUsed", "Data Source is already assigned to a home.");

    public static Error DataSourceAlreadyAssigned =
        Error.Conflict("DataSourceAlreadyAssigned", "Data source is already assigned in home. Cannot override");
}
