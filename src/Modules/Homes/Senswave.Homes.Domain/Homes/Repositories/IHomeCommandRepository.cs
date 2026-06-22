using Senswave.Homes.Domain.Homes.Entities;

namespace Senswave.Homes.Domain.Homes.Repositories;

public interface IHomeCommandRepository
{
    Task<Result> CreateHome(Home home, CancellationToken cancellationToken);
    Task<Result> UpdateHome(Home home, CancellationToken cancellationToken);
    Task<Result> RemoveHome(Guid homeId, CancellationToken cancellationToken);

    Task<Result> AssignDataSourceToHome(Home home, Guid dataSourceId, CancellationToken cancellationToken);
    Task<Result> DeleteDataSourceFromHome(Home home, CancellationToken cancellationToken);

    Task<Home?> GetHome(Guid homeId, CancellationToken cancellationToken);

    Task<bool> HomeExists(Guid userId, string name, CancellationToken cancellationToken);
    Task<int> CountHomeByDataSourceId(Guid dataSourceId, CancellationToken cancellationToken);
}