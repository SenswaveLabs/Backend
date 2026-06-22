using Senswave.Abstractions.Resulting;
using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Operations.Enums;
using Senswave.Devices.Domain.Operations.Factory;
using Senswave.Devices.Domain.Operations.Types;
using Senswave.Devices.Domain.Operations.ValueObjects;
using Senswave.Devices.Domain.Widgets.Entities;
using Senswave.Devices.Domain.Widgets.Factory;
using Senswave.Devices.Domain.Widgets.Repositories;
using Senswave.Devices.Domain.Widgets.Types;
using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Widgets;

public abstract class BaseWidgetTests
{
    public OperationFactory operationFactory;

    public WidgetFactory widgetFactory;

    public Mock<IWidgetQueryRepository> widgetQueryRepository;

    protected BaseWidgetTests()
    {
        var operationFactoryLogger = new Mock<ILogger<OperationFactory>>();
        var operationLogger = new Mock<ILogger<IOperation>>();
        operationFactory = new OperationFactory(operationLogger.Object, operationFactoryLogger.Object);

        widgetQueryRepository = new Mock<IWidgetQueryRepository>();
        var widgetLogger = new Mock<ILogger<WidgetFactory>>();
        var widgetInternalLogger = new Mock<ILogger<IWidget>>();
        widgetFactory = new WidgetFactory(operationFactory, widgetQueryRepository.Object, widgetLogger.Object, widgetInternalLogger.Object);
    }

    protected async Task<Result> BaseValidationTest(Widget widget)
    {
        //Arrange
        widgetQueryRepository.Setup(x => x.GetWidgetWithOperation(widget.Id, default))
            .ReturnsAsync(widget);

        // Act
        var createdWidget = await widgetFactory.Create(widget.Id, default);
        var result = await createdWidget.Data.Validate();

        return result;
    }

    protected Operation BooleanOperation = new()
    {
        DataReferenceId = Guid.NewGuid(),
        DeviceId = Guid.NewGuid(),

        Id = Guid.NewGuid(),
        Configuration = new()
        {
            ["jsonNames"] = new JsonArray("slider"),
            ["isJson"] = true,
        },
        Name = "TestOperation",

        Values =
        [
            new OperationValue
            {
                InternalValue = new()
                {
                    ["value"] = false
                },
                ProcessedAtUtc = DateTime.UtcNow.AddSeconds(-2)
            },
            new OperationValue
            {
                InternalValue = new()
                {
                    ["value"] = false
                },
                ProcessedAtUtc = DateTime.UtcNow.AddSeconds(-1)
            },
            new OperationValue
            {
                InternalValue = new()
                {
                    ["value"] = true
                },
                ProcessedAtUtc = DateTime.UtcNow
            }
        ],

        Type = OperationType.Boolean,
    };

    protected Operation TextOperation = new()
    {
        DataReferenceId = Guid.NewGuid(),
        DeviceId = Guid.NewGuid(),

        Id = Guid.NewGuid(),
        Configuration = new()
        {
            ["jsonNames"] = new JsonArray("slider"),
            ["isJson"] = true,
        },
        Name = "TestOperation",

        Values =
        [
            new OperationValue
            {
                InternalValue = new()
                {
                    ["value"] = "1"
                },
                ProcessedAtUtc = DateTime.UtcNow.AddSeconds(-2)
            },
            new OperationValue
            {
                InternalValue = new()
                {
                    ["value"] = "2"
                },
                ProcessedAtUtc = DateTime.UtcNow.AddSeconds(-1)
            },
            new OperationValue
            {
                InternalValue = new()
                {
                    ["value"] = "3"
                },
                ProcessedAtUtc = DateTime.UtcNow
            }
        ],

        Type = OperationType.Text,
    };

    protected Operation NumberRangedOperation = new()
    {
        DataReferenceId = Guid.NewGuid(),
        DeviceId = Guid.NewGuid(),

        Id = Guid.NewGuid(),
        Configuration = new()
        {
            ["jsonNames"] = new JsonArray("slider"),
            ["isJson"] = true,
            ["min"] = -100.0,
            ["max"] = 100.0,
        },
        Name = "TestOperation",

        Values =
        [
            new OperationValue
            {
                InternalValue = new()
                {
                    ["value"] = 99.1
                },
                ProcessedAtUtc = DateTime.UtcNow.AddSeconds(-2)
            },
            new OperationValue
            {
                InternalValue = new()
                {
                    ["value"] = 98.6
                },
                ProcessedAtUtc = DateTime.UtcNow.AddSeconds(-1)
            },
            new OperationValue
            {
                InternalValue = new()
                {
                    ["value"] = -10.9
                },
                ProcessedAtUtc = DateTime.UtcNow
            }
        ],

        Type = OperationType.Number,
    };

    protected Operation NumberOperation = new()
    {
        DataReferenceId = Guid.NewGuid(),
        DeviceId = Guid.NewGuid(),

        Id = Guid.NewGuid(),
        Configuration = new()
        {
            ["jsonNames"] = new JsonArray("slider"),
            ["isJson"] = true,
            ["min"] = double.MinValue,
            ["max"] = double.MaxValue,
        },
        Name = "TestOperation",

        Values =
        [
            new OperationValue
                {
                    InternalValue = new()
                    {
                        ["value"] = 99.1
                    },
                    ProcessedAtUtc = DateTime.UtcNow.AddSeconds(-2)
                },
                new OperationValue
                {
                    InternalValue = new()
                    {
                        ["value"] = 98.6
                    },
                    ProcessedAtUtc = DateTime.UtcNow.AddSeconds(-1)
                },
                new OperationValue
                {
                    InternalValue = new()
                    {
                        ["value"] = -10.9
                    },
                    ProcessedAtUtc = DateTime.UtcNow
                }
        ],

        Type = OperationType.Number,
    };

    protected Operation IntegerRangedOperation = new()
    {
        DataReferenceId = Guid.NewGuid(),
        DeviceId = Guid.NewGuid(),

        Id = Guid.NewGuid(),
        Configuration = new()
        {
            ["jsonNames"] = new JsonArray("slider"),
            ["isJson"] = true,
            ["min"] = -100,
            ["max"] = 100,
        },
        Name = "TestOperation",

        Values =
        [
            new OperationValue
            {
                InternalValue = new()
                {
                    ["value"] = 99
                },
                ProcessedAtUtc = DateTime.UtcNow.AddSeconds(-2)
            },
            new OperationValue
            {
                InternalValue = new()
                {
                    ["value"] = 44
                },
                ProcessedAtUtc = DateTime.UtcNow.AddSeconds(-1)
            },
            new OperationValue
            {
                InternalValue = new()
                {
                    ["value"] = -10
                },
                ProcessedAtUtc = DateTime.UtcNow
            }
        ],

        Type = OperationType.Integer,
    };

    protected Operation IntegerOperation = new()
    {
        DataReferenceId = Guid.NewGuid(),
        DeviceId = Guid.NewGuid(),

        Id = Guid.NewGuid(),
        Configuration = new()
        {
            ["jsonNames"] = new JsonArray("slider"),
            ["isJson"] = true,
            ["min"] = int.MinValue,
            ["max"] = int.MaxValue,
        },
        Name = "TestOperation",

        Values =
    [
        new OperationValue
            {
                InternalValue = new()
                {
                    ["value"] = 99
                },
                ProcessedAtUtc = DateTime.UtcNow.AddSeconds(-2)
            },
            new OperationValue
            {
                InternalValue = new()
                {
                    ["value"] = 44
                },
                ProcessedAtUtc = DateTime.UtcNow.AddSeconds(-1)
            },
            new OperationValue
            {
                InternalValue = new()
                {
                    ["value"] = -10
                },
                ProcessedAtUtc = DateTime.UtcNow
            }
    ],

        Type = OperationType.Integer,
    };

    protected Operation OptionsOperation = new()
    {
        DataReferenceId = Guid.NewGuid(),
        DeviceId = Guid.NewGuid(),
        Id = Guid.NewGuid(),


        Configuration = new()
        {
            ["jsonNames"] = new JsonArray("radio"),
            ["isJson"] = true,
            ["options"] = new JsonArray(
                new JsonObject
                {
                    ["value"] = 1,
                    ["name"] = "Option1"
                },
                new JsonObject
                {
                    ["value"] = 2,
                    ["name"] = "Option2"
                },
                new JsonObject
                {
                    ["value"] = 3,
                    ["name"] = "Option3"
                },
                new JsonObject
                {
                    ["value"] = "test",
                    ["name"] = "Option4"
                }

            )
        },
        Name = "TestOperation",
        Type = OperationType.Options,
        Values =
        [
            new OperationValue
            {
                InternalValue = new()
                {
                    ["value"] = "Option1"
                },
                ProcessedAtUtc = DateTime.UtcNow.AddSeconds(-2)
            },
            new OperationValue
            {
                InternalValue = new()
                {
                    ["value"] = "Option2"
                },
                ProcessedAtUtc = DateTime.UtcNow.AddSeconds(-1)
            },
            new OperationValue
            {
                InternalValue = new()
                {
                    ["value"] = "Option3"
                },
                ProcessedAtUtc = DateTime.UtcNow
            }
        ],
    };

    protected Operation HexColorOperation = new()
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

        Values =
        [
            new OperationValue
            {
                InternalValue = new()
                {
                    ["value"] = "#F0F"
                },
                ProcessedAtUtc = DateTime.UtcNow.AddSeconds(-2)
            },
            new OperationValue
            {
                InternalValue = new()
                {
                    ["value"] = "#000"
                },
                ProcessedAtUtc = DateTime.UtcNow.AddSeconds(-1)
            },
            new OperationValue
            {
                InternalValue = new()
                {
                    ["value"] = "#FFFFFF"
                },
                ProcessedAtUtc = DateTime.UtcNow
            }
        ],

        Type = OperationType.HexColor,
    };
}
