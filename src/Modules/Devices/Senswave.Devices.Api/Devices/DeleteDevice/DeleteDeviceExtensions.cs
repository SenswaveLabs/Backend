using Senswave.Devices.Application.Devices.Features.DeleteDevice;

namespace Senswave.Devices.Api.Devices.DeleteDevice;

internal static class DeleteDeviceExtensions
{
    internal static DeleteDeviceCommand ToDeleteDeviceCommand(this Guid deviceId, Guid userId) => new()
    {
        DeviceId = deviceId,
        UserId = userId
    };
}
