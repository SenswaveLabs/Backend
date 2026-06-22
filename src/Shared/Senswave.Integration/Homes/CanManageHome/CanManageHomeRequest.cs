namespace Senswave.Integration.Homes.CanManageHome;

public record CanManageHomeRequest
{
    public Guid UserId { get; set; }
    public Guid HomeId { get; set; }
}
