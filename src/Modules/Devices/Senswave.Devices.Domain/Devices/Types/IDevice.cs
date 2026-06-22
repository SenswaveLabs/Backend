using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Devices.Models;
using Senswave.Devices.Domain.Devices.TileTypes;

namespace Senswave.Devices.Domain.Devices.Types;

public interface IDevice
{
    Guid Id { get; }

    IDeviceTile Tile { get; }

    Task<Result<DisplayDeviceModel>> ToDisplay();

    Device AsEntity();
}
