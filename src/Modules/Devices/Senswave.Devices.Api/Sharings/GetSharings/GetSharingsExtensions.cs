using Senswave.Abstractions.Resulting;
using Senswave.Devices.Application.ShareDevices.Features.GetDeviceSharings;
using Senswave.Devices.Domain.ShareDevices.Extensions;

namespace Senswave.Devices.Api.Sharings.GetSharings;

internal static class GetSharingsExtensions
{
    public static GetSharingsResponse ToResponse(this Result<List<DeviceSharingModel>> result) => new()
    {
        Items = [.. result.Data.Select(x => x.ToDto())]
    };

    public static SharingDto ToDto(this DeviceSharingModel model) => new()
    {
        SharingId = model.SharingId,
        FriendEmail = model.FriendEmail,
        SharingType = model.SharingType.FromDeviceSharingType()
    };
}
