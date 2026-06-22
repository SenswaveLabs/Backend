using Senswave.Homes.Application.Homes.Features.DeleteHomeDataSource;

namespace Senswave.Homes.Api.Homes.Features.DeleteHomeDataSource;

internal static class DeleteHomeDataSourceExtensions
{
    public static DeleteHomeDataSourceCommand ToDeleteHomeDataSourceCommand(this Guid homeId, Guid userId) => new()
    {
        UserId = userId,
        HomeId = homeId,
    };
}
