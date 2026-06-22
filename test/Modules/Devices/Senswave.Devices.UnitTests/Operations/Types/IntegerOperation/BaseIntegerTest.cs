using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Operations.Enums;
using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Operations.Types.IntegerOperation;

public class BaseIntegerTest : BaseOperationTest
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
        Type = OperationType.Integer,
    };

    protected Operation RangedOperation = new()
    {
        DataReferenceId = Guid.NewGuid(),
        DeviceId = Guid.NewGuid(),

        Id = Guid.NewGuid(),
        Configuration = new()
        {
            ["isJson"] = false,
            ["min"] = -100,
            ["max"] = 100,
        },
        Name = "TestOperation",
        Type = OperationType.Integer,
    };

    protected Operation JsonOperation = new()
    {
        DataReferenceId = Guid.NewGuid(),
        DeviceId = Guid.NewGuid(),

        Id = Guid.NewGuid(),
        Configuration = new()
        {
            ["jsonNames"] = new JsonArray("values"),
            ["isJson"] = true,
        },
        Name = "TestOperation",
        Type = OperationType.Integer,
    };

    protected Operation JsonRangedOperation = new()
    {
        DataReferenceId = Guid.NewGuid(),
        DeviceId = Guid.NewGuid(),

        Id = Guid.NewGuid(),
        Configuration = new()
        {
            ["jsonNames"] = new JsonArray("detection"),
            ["isJson"] = true,
            ["min"] = -100,
            ["max"] = 100,
        },
        Name = "TestOperation",
        Type = OperationType.Integer,
    };
}
