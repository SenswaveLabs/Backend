using Senswave.Devices.Application.Devices.Features.DeviceTileAction;

namespace Senswave.Devices.Api.Devices.DeviceTileAction;

internal static class DeviceTileActionExtensions
{
    internal static DeviceTileActionCommand ToCommand(this DeviceTileActionRequest dto, Guid userId, Guid deviceId) => new()
    {
        DeviceId = deviceId,
        UserId = userId,
        Value = dto.Value
    };
}
