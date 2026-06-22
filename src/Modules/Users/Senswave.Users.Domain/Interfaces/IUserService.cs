using Senswave.Abstractions.Resulting;
using Senswave.Users.Domain.Entity;
using Senswave.Users.Domain.Enums;

namespace Senswave.Users.Domain.Interfaces;

public interface IUserService
{
    Task<Result> UserHasLatestConsents(Guid userId, CancellationToken cancellationToken = default);

    Task<Result<User>> Register(ExternalProvider provider, string email, string subject, CancellationToken cancellationToken = default);

    Task<Result> LinkExtenralProvider(ExternalProvider provider, User user, string subject, CancellationToken cancellationToken = default);
}
