namespace Senswave.Homes.Api.Sharings.CreateSharing;

public class CreateSharingRequest
{
    public Guid HomeId { get; set; }
    public string FriendEmail { get; set; } = string.Empty;

    public string SharingType { get; set; } = string.Empty;
}