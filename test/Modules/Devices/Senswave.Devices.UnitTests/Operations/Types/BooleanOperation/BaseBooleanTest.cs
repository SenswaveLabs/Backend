using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Operations.Enums;
using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Operations.Types.BooleanOperation;

public class BaseBooleanTest : BaseOperationTest
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
        Type = OperationType.Boolean,
    };

    protected Operation JsonOperation = new()
    {
        DataReferenceId = Guid.NewGuid(),
        DeviceId = Guid.NewGuid(),

        Id = Guid.NewGuid(),
        Configuration = new()
        {
            ["jsonNames"] = new JsonArray("value1234"),
            ["isJson"] = true,
        },
        Name = "TestOperation",
        Type = OperationType.Boolean,
    };
}
