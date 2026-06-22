using Senswave.Homes.Domain.Homes.Entities;

namespace Senswave.Homes.Application.Homes.Features.GetHomes;

public class GetHomesQuery : IPagedQuery<IEnumerable<Home>>
{
    public int Page { get; set; }
    public int Size { get; set; }
    public Guid UserId { get; set; } = Guid.Empty;
}