using Senswave.Users.Domain.Entity;

namespace Senswave.Users.Domain.Repositories;

public interface IQueryLegalRepository
{
    Task<PrivacyPolicy> GetLatestPrivacyPolicy(CancellationToken cancellationToken);

    Task<TermsAndConditions> GetLatestTermsAndConditions(CancellationToken cancellationToken);
}
