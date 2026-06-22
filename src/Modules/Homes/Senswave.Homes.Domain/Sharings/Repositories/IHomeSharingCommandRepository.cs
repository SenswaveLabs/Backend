using Senswave.Homes.Domain.Sharings.Entities;

namespace Senswave.Homes.Domain.Sharings.Repositories;

public interface IHomeSharingCommandRepository
{
    Task<Result> CreateHomeSharing(HomeSharing homeSharing, CancellationToken cancellationToken);

    Task<Result> DeleteSharing(HomeSharing homeSharing, CancellationToken cancellationToken);

    Task<Result> CreateHomeSharingInvitation(HomeSharingInvitation invitation, CancellationToken cancellationToken);

    Task<HomeSharing?> GetHomeSharing(Guid homeSharingId, CancellationToken cancellationToken);

    Task<HomeSharing?> GetHomeSharingForUser(Guid homeId, Guid userId, CancellationToken cancellationToken);

    Task<List<HomeSharingInvitation>> GetInvitationsByUser(Guid userId, CancellationToken cancellationToken);

    Task<Result> DeleteSharingInvitation(HomeSharingInvitation invitationAlreadyExists, CancellationToken cancellationToken);
}
