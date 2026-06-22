using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Operations.Types;
using System.Text.Json;

namespace Senswave.Devices.Domain.Devices.TileTypes.Display;

internal static class DisplayDeviceTileExtensions
{
    private static readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };

    internal static DisplayDeviceTile ToDisplayDeviceTile(
        this DeviceTile tile, IOperation operation, ILogger<IDeviceTile> logger)
    {
        var configuration = JsonSerializer.Deserialize<DisplayDeviceTileConfiguration>(tile.Configuration, _options)
            ?? new DisplayDeviceTileConfiguration();

        return new DisplayDeviceTile(tile, configuration, operation, logger);
    }
}
