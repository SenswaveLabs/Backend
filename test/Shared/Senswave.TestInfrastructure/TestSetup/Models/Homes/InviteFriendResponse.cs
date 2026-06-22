namespace Senswave.TestInfrastructure.TestSetup.Models.Homes;

public class InviteFriendResponse
{
    [JsonPropertyName("invitationId")]
    public Guid InvitationId { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;

    [JsonPropertyName("createdUtc")]
    public DateTime CreatedUtc { get; set; }
}