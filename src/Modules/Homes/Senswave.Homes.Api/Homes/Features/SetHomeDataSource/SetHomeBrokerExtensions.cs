using Senswave.Homes.Application.Homes.Features.AssignHomeDataSource;

namespace Senswave.Homes.Api.Homes.Features.SetHomeDataSource;

public static class SetHomeBrokerExtensions
{
    public static AssignHomeDataSourceCommand ToCommand(this AssignHomeDataSourceRequest putHomeBroker, Guid userId, Guid homeId) => new()
    {
        UserId = userId,
        HomeId = homeId,
        DataSourceId = putHomeBroker.BrokerId
    };
}