using Senswave.Integration.Shared;

namespace Senswave.Integration.DataSource.State;

public partial record DataSourceStateResponse : BaseInternalResponse
{
    public string State { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
