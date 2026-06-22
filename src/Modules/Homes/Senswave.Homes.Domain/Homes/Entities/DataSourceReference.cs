namespace Senswave.Homes.Domain.Homes.Entities;

public class DataSourceReference : Entity
{
    public Guid HomeId { get; set; }

    public Home Home { get; set; }

    public Guid DataSourceId { get; set; }
}
