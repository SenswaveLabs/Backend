namespace Senswave.Homes.Api.Homes.Features.CreateHome;

public record CreateHomeRequest
{
    public Guid? DataSourceId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }
}