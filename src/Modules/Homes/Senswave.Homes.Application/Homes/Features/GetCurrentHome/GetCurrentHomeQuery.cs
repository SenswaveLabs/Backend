using Senswave.Homes.Domain.Homes.Entities;

namespace Senswave.Homes.Application.Homes.Features.GetCurrentHome;

public class GetCurrentHomeQuery : IQuery<Home>
{
    public Guid UserId { get; set; } = Guid.Empty;

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }
}