namespace Senswave.TestInfrastructure.TestSetup.Models.Devices;

public class GetDeviceTestResponse
{
    [JsonPropertyName("items")]
    public List<GetDeviceItem> Items { get; set; } = [];

    public class GetDeviceItem
    {
        [JsonPropertyName("sharingId")]
        public Guid? SharingId { get; set; }

        [JsonPropertyName("friendEmail")]
        public string FriendEmail { get; set; } = string.Empty;

        [JsonPropertyName("sharingType")]
        public string SharingType { get; set; } = string.Empty;
    }
}