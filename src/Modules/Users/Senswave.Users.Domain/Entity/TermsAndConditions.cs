using Senswave.Abstractions.Entities;

namespace Senswave.Users.Domain.Entity;

public sealed class TermsAndConditions : AuditableEntity
{
    public string Version { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public List<UserConsents> UserConsents { get; set; } = [];
}