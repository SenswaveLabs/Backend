using Senswave.Abstractions.Resulting;
using Senswave.Users.Domain.Entity;

namespace Senswave.Users.Domain.Repositories;

public interface ICommandUserRepository
{
    Task<Result> UpdateUser(User user, CancellationToken cancellationToken);

    Task<Result> CreateUserConsents(UserConsents consents, CancellationToken cancellationToken);

    Task<User?> GetUserWithConsents(Guid userId, CancellationToken cancellation);

    Task<User?> GetUser(Guid userId, CancellationToken cancellation);

    Task<Result> RemoveUser(User user, RemovedUser removedUser, CancellationToken cancellationToken);
}
