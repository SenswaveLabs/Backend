using Senswave.Abstractions.Resulting;
using Senswave.Homes.Application.Sharings.Features.CreateSharing;
using Senswave.Homes.Domain.Sharings.Extensions;

namespace Senswave.Homes.Api.Sharings.CreateSharing;

internal static class CreateSharingExtensions
{
    public static CreateSharingCommand ToCommand(this CreateSharingRequest invitationDto, Guid ownerId) => new()
    {
        OwnerId = ownerId,
        HomeId = invitationDto.HomeId,
        FriendEmail = invitationDto.FriendEmail,
        SharingType = invitationDto.SharingType.ToHomeSharingType()
    };

    public static HomeSharingCreatedResponse ToResponse(this Result<CreateSharingDto> result) => new()
    {
        InvitationId = result.Data.Id,
        Password = result.Data.Password,
        CreatedUtc = result.Data.CreatedAtUtc,
        ExpiresAtUtc = result.Data.ExpiresAtUtc
    };
}
