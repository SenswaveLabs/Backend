using Senswave.Users.Domain.Entity;

namespace Senswave.Users.Domain.Repositories;

public interface IQueryUserRepository
{
    Task<User?> GetUserWithConsentsAndLegal(Guid id, CancellationToken cancellation);

    Task<User?> GetUserByEmail(string email, CancellationToken cancellationToken);

    Task<IList<User>> GetUsersByIds(IList<Guid> ids, CancellationToken cancellationToken);
}
