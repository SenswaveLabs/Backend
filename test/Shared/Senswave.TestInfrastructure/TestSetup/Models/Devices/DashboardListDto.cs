using Senswave.TestInfrastructure.TestSetup.Models.Common;

namespace Senswave.TestInfrastructure.TestSetup.Models.Devices;

public class DashboardListDto
{
    [JsonPropertyName("items")]
    public IEnumerable<IdResponse> Dashboards { get; set; } = [];
}
