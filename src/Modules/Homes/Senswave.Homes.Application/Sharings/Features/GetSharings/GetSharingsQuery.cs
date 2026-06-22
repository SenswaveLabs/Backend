namespace Senswave.Homes.Application.Sharings.Features.GetSharings;

public class GetSharingsQuery : IQuery<IList<SharingModel>>
{
    public Guid UserId { get; set; } = Guid.Empty;

    public Guid HomeId { get; set; } = Guid.Empty;
}