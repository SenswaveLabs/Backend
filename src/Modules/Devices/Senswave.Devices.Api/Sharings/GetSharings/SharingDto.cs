namespace Senswave.Devices.Api.Sharings.GetSharings;

public record SharingDto
{
    public Guid? SharingId { get; set; }

    public string FriendEmail { get; set; } = string.Empty;

    public string SharingType { get; set; } = string.Empty;
}
