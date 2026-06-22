using Senswave.Abstractions.Entities;

namespace Senswave.Users.Domain.Entity;

public class UserConsents : AuditableEntity
{
    public Guid UserId { get; set; }

    public User? User { get; set; }

    public Guid TermsAndConditionsId { get; set; }

    public TermsAndConditions? TermsAndConditions { get; set; }

    public Guid PrivacyPolicyId { get; set; }

    public PrivacyPolicy? PrivacyPolicy { get; set; }
}
