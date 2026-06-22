using Senswave.Homes.Application.Sharings.Features.AcceptSharing;

namespace Senswave.Homes.Api.Sharings.AcceptSharing;

internal static class AcceptSharingExtensions
{
    internal static AcceptSharingCommand ToCommand(this AcceptSharingRequest request, Guid userId) => new()
    {
        UserId = userId,
        Password = request.Password
    };
}
