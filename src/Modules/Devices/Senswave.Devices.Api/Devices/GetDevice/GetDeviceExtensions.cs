using Senswave.Abstractions.Resulting;
using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Devices.Enums;
using Senswave.Devices.Domain.Devices.Extensions;

namespace Senswave.Devices.Api.Devices.GetDevice;

internal static class GetDeviceExtensions
{
    public static DeviceDto ToResponse(this Result<Device> result) => new()
    {
        Id = result.Data!.Id,
        Icon = result.Data.Icon,
        Name = result.Data.Name,
        Tile = new DeviceTileDto
        {
            Type = result.Data.Tile.Type.FromDeviceTileType(),
            OperationId = result.Data.Tile.SwitchOperationId?.ToString(),
            DisplayableOperationId = result.Data.Tile.DisplayableOperationId?.ToString(),
            Configuration = result.Data.Tile.Configuration,
        },
        Presence = new DevicePresenceDto
        {
            Type = (result.Data.Presence?.Type ?? DevicePresenceType.Default).FromDevicePresenceType(),
            OperationId = result.Data.Presence?.OperationId?.ToString(),
        },
        RoomId = result.Data.RoomReferenceId
    };
}
