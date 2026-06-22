using Senswave.Integration.Shared;

namespace Senswave.Integration.User;

public record UserByEmailResponse : BaseInternalResponse
{
    public Guid UserId { get; set; } = Guid.Empty;
}