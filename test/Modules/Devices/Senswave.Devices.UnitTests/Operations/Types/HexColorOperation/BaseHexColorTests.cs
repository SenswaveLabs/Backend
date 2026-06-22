using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Operations.Enums;
using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Operations.Types.HexColorOperation;

public abstract class BaseHexColorTests : BaseOperationTest
{
    protected Operation Operation = new()
    {
        DataReferenceId = Guid.NewGuid(),
        DeviceId = Guid.NewGuid(),

        Id = Guid.NewGuid(),
        Configuration = new()
        {
            ["isJson"] = false,
        },
        Name = "TestOperation",
        Type = OperationType.HexColor,
    };

    protected Operation JsonOperation = new()
    {
        DataReferenceId = Guid.NewGuid(),
        DeviceId = Guid.NewGuid(),

        Id = Guid.NewGuid(),
        Configuration = new()
        {
            ["jsonNames"] = new JsonArray("color"),
            ["isJson"] = true,
        },
        Name = "TestOperation",
        Type = OperationType.HexColor,
    };
}
