using Senswave.Abstractions.Resulting;
using Senswave.Devices.Domain.Devices.Enums;
using Senswave.Devices.Domain.Devices.Extensions;
using Senswave.Devices.Domain.Devices.Models;

namespace Senswave.Devices.Api.Devices.DisplayDevice;

internal static class DsiplayDeviceExtensions
{
    internal static DisplayDeviceDto ToDto(this DisplayDeviceModel model) => new()
    {
        Id = model.Id,
        RoomId = model.RoomId,
        Icon = model.Icon,
        Name = model.Name,

        Tile = new DeviceTileDto()
        {
            Type = model.Tile.Type.FromDeviceTileType(),
            Value = model.Tile.Value,
            Configuration = model.Tile.Configuration,
        },

        Presence = new DevicePresenceDto()
        {
            Type = model.Presence.Type.FromDevicePresenceType(),
            Value = model.Presence.Type == DevicePresenceType.BooleanOperation ? model.Presence.Value : null,
            LastSeenAtUtc = model.Presence.Type == DevicePresenceType.BooleanOperation ? model.Presence.LastSeenAtUtc : null,
        },

        CreatedAtUtc = model.CreatedAtUtc,
        UpdatedAtUtc = model.UpdatedAtUtc,
    };

    internal static DisplayDeviceDto ToDto(this Result<DisplayDeviceModel> result) => result.Data.ToDto();
}
