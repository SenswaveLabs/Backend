using Microsoft.EntityFrameworkCore;
using Senswave.Users.Domain.Entity;
using Senswave.Users.Domain.Repositories;

namespace Senswave.Users.Infrastructure.Repositories;

internal sealed class QueryLegalRepository(UsersContext context) : IQueryLegalRepository
{
    public Task<PrivacyPolicy> GetLatestPrivacyPolicy(CancellationToken cancellationToken) => context.PrivacyPolicies
        .OrderByDescending(x => x.CreatedAtUtc)
        .AsNoTracking()
        .FirstAsync(cancellationToken);

    public Task<TermsAndConditions> GetLatestTermsAndConditions(CancellationToken cancellationToken) => context.TermsAndConditions
        .OrderByDescending(x => x.CreatedAtUtc)
        .AsNoTracking()
        .FirstAsync(cancellationToken);

}
