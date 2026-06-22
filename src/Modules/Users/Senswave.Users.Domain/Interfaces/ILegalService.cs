using Senswave.Abstractions.Resulting;
using Senswave.Users.Domain.Entity;

namespace Senswave.Users.Domain.Interfaces;

public interface ILegalService
{
    Task<Result<TermsAndConditions>> GetTermsAndConditions(CancellationToken cancellationToken);

    Task<Result<PrivacyPolicy>> GetPrivacyPolicy(CancellationToken cancellationToken);

    Task<bool> VerifyLegal(CancellationToken cancellationToken = default);
}
