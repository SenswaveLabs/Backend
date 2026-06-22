using Senswave.Homes.Api.Homes.Features.GetHome;

namespace Senswave.TestInfrastructure.TestSetup.Models.Homes;

public class GetHomeResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("dataSourceId")]
    public Guid? DataSourceId { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("icon")]
    public string Icon { get; init; } = string.Empty;

    [JsonPropertyName("location")]
    public LocationTestDto Location { get; set; } = new LocationTestDto();

    [JsonPropertyName("rooms")]
    public ICollection<RoomDto> Rooms { get; set; } = [];
}