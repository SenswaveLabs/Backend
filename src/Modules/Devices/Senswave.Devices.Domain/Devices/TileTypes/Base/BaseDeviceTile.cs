using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Devices.Enums;
using Senswave.Devices.Domain.Devices.Models;

namespace Senswave.Devices.Domain.Devices.TileTypes.Base;

public abstract class BaseDeviceTile(DeviceTile tile) : IDeviceTile
{
    protected readonly DeviceTile _tile = tile;

    public Guid Id => _tile.Id;

    public Guid DeviceId => _tile.DeviceId;

    public DeviceTileType Type => _tile.Type;

    public abstract Task<Result> Validate();

    public abstract Task<Result<DeviceTileMessageModel>> Preprocess(JsonNode value);

    public abstract Result<DisplayDeviceTileModel> ToDisplay();

    public abstract DeviceTile AsEntity();
}
