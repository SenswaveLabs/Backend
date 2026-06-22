namespace Senswave.Homes.Api.Homes.Features.GetHomes;

public record GetHomesResponse
{
    public IList<HomeDto> Items { get; set; } = [];
}
