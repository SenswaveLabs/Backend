using Senswave.Homes.Application.Sharings.Features.LeaveSharing;

namespace Senswave.Homes.Api.Sharings.LeaveSharing;

internal static class LeaveSharingExtensions
{
    internal static LeaveSharingCommand ToLeaveSharingCommand(this Guid homeId, IRequestContext context) => new()
    {
        HomeId = homeId,
        UserId = context.UserId
    };
}
