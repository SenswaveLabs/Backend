using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Devices.Enums;
using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Operations.Enums;
using Senswave.Devices.Domain.Operations.ValueObjects;

namespace Senswave.Devices.UnitTests.Devices.TileTypes.Display;

public abstract class BaseDisplayTileTests : BaseTileTest
{
    protected Device NumberDisplayDevice(IList<OperationValue>? values = null, string? unit = null) => new()
    {
        Id = Guid.NewGuid(),
        Tile = new DeviceTile
        {
            Type = DeviceTileType.Display,
            Configuration = unit is not null ? new() { ["unit"] = unit } : [],
            DisplayableOperation = new Operation
            {
                Type = OperationType.Number,
                Values = values?.ToList() ?? []
            }
        },
        CreatedAtUtc = DateTime.UtcNow,
        UpdatedAtUtc = DateTime.UtcNow
    };

    protected Device IntegerDisplayDevice(IList<OperationValue>? values = null, string? unit = null) => new()
    {
        Id = Guid.NewGuid(),
        Tile = new DeviceTile
        {
            Type = DeviceTileType.Display,
            Configuration = unit is not null ? new() { ["unit"] = unit } : [],
            DisplayableOperation = new Operation
            {
                Type = OperationType.Integer,
                Values = values?.ToList() ?? []
            }
        },
        CreatedAtUtc = DateTime.UtcNow,
        UpdatedAtUtc = DateTime.UtcNow
    };
}
