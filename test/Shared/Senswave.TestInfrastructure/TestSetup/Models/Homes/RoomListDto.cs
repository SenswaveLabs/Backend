using Senswave.TestInfrastructure.TestSetup.Models.Common;

namespace Senswave.TestInfrastructure.TestSetup.Models.Homes;

public class RoomListDto
{
    [JsonPropertyName("items")]
    public IdResponse[] Rooms { get; set; } = [];
}
