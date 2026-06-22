namespace Senswave.Devices.Api.Sharings.SetSharing;

public class SetSharingRequest
{
    public Guid DeviceId { get; set; }
    public string SharingType { get; set; } = string.Empty;
    public string FriendEmail { get; set; } = string.Empty;
}