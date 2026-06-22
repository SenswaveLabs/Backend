using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Operations.Enums;
using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Operations.Types.OptionOperation;

public abstract class BaseOptionTest : BaseOperationTest
{
    protected Operation Operation = new()
    {
        DataReferenceId = Guid.NewGuid(),
        DeviceId = Guid.NewGuid(),

        Id = Guid.NewGuid(),
        Configuration = new()
        {
            ["isJson"] = false,
            ["options"] = new JsonArray(
                new JsonObject
                {
                    ["name"] = "Option1",
                    ["value"] = "Value"
                },
                new JsonObject
                {
                    ["name"] = "Option2",
                    ["value"] = 12.31
                },
                new JsonObject
                {
                    ["name"] = "Option3",
                    ["value"] = 1
                },
                new JsonObject
                {
                    ["name"] = "Option4",
                    ["value"] = true
                }
            ),
        },

        Name = "TestOperation",
        Type = OperationType.Options,
    };

    protected Operation JsonOperation = new()
    {
        DataReferenceId = Guid.NewGuid(),
        DeviceId = Guid.NewGuid(),
        Id = Guid.NewGuid(),
        Configuration = new JsonObject
        {
            ["jsonNames"] = new JsonArray("values"),
            ["isJson"] = true,
            ["options"] = new JsonArray(
                new JsonObject
                {
                    ["name"] = "Option1",
                    ["value"] = "Value"
                },
                new JsonObject
                {
                    ["name"] = "Option2",
                    ["value"] = 12.31
                },
                new JsonObject
                {
                    ["name"] = "Option3",
                    ["value"] = 1
                },
                new JsonObject
                {
                    ["name"] = "Option4",
                    ["value"] = true
                }
            ),
        },

        Name = "TestOperation",
        Type = OperationType.Options,
    };
}
