using Senswave.Devices.Application.Devices.Features.UpdateDevice;
using Senswave.Devices.Domain.Devices.Extensions;

namespace Senswave.Devices.Api.Devices.UpdateDevice;


internal static class UpdateDeviceExtensions
{
    public static UpdateDeviceCommand ToCommand(this UpdateDeviceRequest dto, Guid userId, Guid deviceID) => new()
    {
        DeviceId = deviceID,
        UserId = userId,
        RoomId = dto.RoomId,

        Icon = dto.Icon,
        Name = dto.Name,

        TileOperationId = dto.OperationId,
        TileDisplayableOperationId = dto.DisplayableOperationId,
        TileConfiguration = dto.Configuration,
        TileType = dto.Type.ToDeviceTileType(),

        PresenceOperationId = dto.PresenceOperationId,
        PresenceType = dto.PresenceType.ToDevicePresenceType()
    };
}
