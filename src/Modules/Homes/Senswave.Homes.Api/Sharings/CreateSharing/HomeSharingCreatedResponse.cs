namespace Senswave.Homes.Api.Sharings.CreateSharing;

public class HomeSharingCreatedResponse
{
    public Guid InvitationId { get; set; }

    public string Password { get; set; } = string.Empty;

    public DateTime ExpiresAtUtc { get; set; }

    public DateTime CreatedUtc { get; set; }
}