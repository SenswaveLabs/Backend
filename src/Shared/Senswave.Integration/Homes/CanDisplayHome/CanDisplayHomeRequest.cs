namespace Senswave.Integration.Homes.CanDisplayHome;

public record CanDisplayHomeRequest
{
    public Guid UserId { get; set; }
    public Guid HomeId { get; set; }
}
