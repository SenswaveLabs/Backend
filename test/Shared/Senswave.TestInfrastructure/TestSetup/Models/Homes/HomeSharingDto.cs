namespace Senswave.TestInfrastructure.TestSetup.Models.Homes;

public record HomeSharingDto
{
    [JsonPropertyName("sharingId")]
    public Guid SharingId { get; set; }

    [JsonPropertyName("friendEmail")]
    public string FriendEmail { get; set; } = string.Empty;

    [JsonPropertyName("sharingType")]
    public string SharingType { get; set; } = string.Empty;
}