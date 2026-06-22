namespace Senswave.Homes.Api.Homes.Features.GetHome;

public class DataSourceDto
{
    public Guid? Id { get; init; }

    public string State { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;
}
