using Senswave.Homes.Application.Sharings.Features.DeleteSharing;

namespace Senswave.Homes.Api.Sharings.DeleteSharing;

internal static class DeleteSharingExtensions
{
    internal static DeleteSharingCommand ToDeleteSharingCommand(this Guid sharingId, Guid userId) => new()
    {
        HomeSharingId = sharingId,
        UserId = userId
    };
}
