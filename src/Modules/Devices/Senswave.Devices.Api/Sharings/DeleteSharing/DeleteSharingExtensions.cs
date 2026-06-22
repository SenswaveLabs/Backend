using Senswave.Devices.Application.ShareDevices.Features.DeleteDeviceSharing;

namespace Senswave.Devices.Api.Sharings.DeleteSharing;

internal static class DeleteSharingExtensions
{
    internal static DeleteDeviceSharingCommand ToDeleteDeviceSharingsCommand(this Guid deviceSharingId, Guid userId) => new()
    {
        DeviceSharingId = deviceSharingId,
        UserId = userId
    };
}
