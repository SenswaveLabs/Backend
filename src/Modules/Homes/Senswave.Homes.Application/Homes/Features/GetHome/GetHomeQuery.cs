using Senswave.Homes.Application.Homes.Models;

namespace Senswave.Homes.Application.Homes.Features.GetHome;

public class GetHomeQuery : IQuery<GetHomeModel>
{
    public Guid UserId { get; set; } = Guid.Empty;

    public Guid HomeId { get; set; } = Guid.Empty;
}