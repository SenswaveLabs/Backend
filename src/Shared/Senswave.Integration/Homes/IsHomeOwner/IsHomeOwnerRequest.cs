namespace Senswave.Integration.Homes.IsHomeOwner;

public record IsHomeOwnerRequest
{
    public Guid HomeId { get; set; } = Guid.Empty;

    public Guid UserId { get; set; } = Guid.Empty;
};