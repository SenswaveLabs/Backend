using Senswave.Homes.Domain.Homes.Entities;

namespace Senswave.Homes.Application.Homes.Models;

public class GetHomeModel
{
    public Home Home { get; init; } = new();

    public DataSourceModel? DataSource { get; set; }
}
