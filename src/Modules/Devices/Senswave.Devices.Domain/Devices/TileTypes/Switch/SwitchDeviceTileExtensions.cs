using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Operations.Types;

namespace Senswave.Devices.Domain.Devices.TileTypes.Switch;

internal static class SwitchDeviceTileExtensions
{
    internal static SwitchDeviceTile ToSwitchDeviceTile(
        this DeviceTile tile,
        IOperation operation,
        ILogger<IDeviceTile> logger)
    {
        return new SwitchDeviceTile(tile, new(), operation, logger);
    }
}
