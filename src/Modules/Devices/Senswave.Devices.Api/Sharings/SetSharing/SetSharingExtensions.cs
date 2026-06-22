using Senswave.Devices.Application.ShareDevices.Features.SetDeviceSharing;
using Senswave.Devices.Domain.ShareDevices.Extensions;

namespace Senswave.Devices.Api.Sharings.SetSharing;

internal static class SetSharingExtensions
{
    internal static SetDeviceSharingsCommand ToCommand(this SetSharingRequest request, Guid userId) => new()
    {
        UserId = userId,
        FriendEmail = request.FriendEmail,
        DeviceId = request.DeviceId,
        SharingType = request.SharingType.ToDeviceSharingType()
    };
}
