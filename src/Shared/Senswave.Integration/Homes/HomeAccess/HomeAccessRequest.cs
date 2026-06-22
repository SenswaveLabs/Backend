namespace Senswave.Integration.Homes.HomeAccess;

public record HomeAccessRequest
{
    public Guid UserId { get; set; }
    public Guid HomeId { get; set; }

    public string SharingType { get; set; } = string.Empty;
}
