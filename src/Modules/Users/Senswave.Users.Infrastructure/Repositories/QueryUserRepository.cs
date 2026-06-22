using Microsoft.EntityFrameworkCore;
using Senswave.Users.Domain.Entity;
using Senswave.Users.Domain.Repositories;

namespace Senswave.Users.Infrastructure.Repositories;

public class QueryUserRepository(UsersContext context) : IQueryUserRepository
{
    public Task<User?> GetUserWithConsentsAndLegal(Guid id, CancellationToken cancellation) => context.Users
        .Include(x => x.UserConsents)
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.Id == id, cancellation);

    public Task<User?> GetUserByEmail(string email, CancellationToken cancellationToken) => context.Users
        .Where(x => x.Email == email || x.NormalizedEmail == email.ToUpperInvariant())
        .AsNoTracking()
        .FirstOrDefaultAsync(cancellationToken);

    public async Task<IList<User>> GetUsersByIds(
        IList<Guid> ids,
        CancellationToken cancellationToken) => await context.Users
        .Where(x => ids.Contains(x.Id))
        .AsNoTracking()
        .ToListAsync(cancellationToken);
}
