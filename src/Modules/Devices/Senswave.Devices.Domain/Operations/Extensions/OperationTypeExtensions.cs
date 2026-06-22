using Senswave.Devices.Domain.Operations.Enums;

namespace Senswave.Devices.Domain.Operations.Extensions;

public static class OperationTypeExtensions
{
    public static OperationType ToOperationType(this string type) => type.ToLowerInvariant() switch
    {
        "number" => OperationType.Number,
        "boolean" => OperationType.Boolean,
        "text" => OperationType.Text,
        "integer" => OperationType.Integer,
        "options" => OperationType.Options,
        "hexcolor" => OperationType.HexColor,

        "" => OperationType.Empty,
        _ => OperationType.Invalid
    };

    public static string FromOperationType(this OperationType type) => type switch
    {
        OperationType.Number => "Number",
        OperationType.Boolean => "Boolean",
        OperationType.Text => "Text",
        OperationType.Integer => "Integer",
        OperationType.Options => "Options",
        OperationType.HexColor => "HexColor",

        _ => "",
    };

    public static List<OperationType> GetOperationsExcept(this OperationType type) => Enum
        .GetValues(typeof(OperationType))
        .Cast<OperationType>()
        .Where(x => x != type)
        .ToList();

    public static List<object[]> GetOperationsObjectArrayExcept(this OperationType type) => type
        .GetOperationsExcept()
        .Select(type => new object[] { type })
        .ToList();

    public static List<OperationType> GetOperationsExcept(this List<OperationType> types) => Enum
        .GetValues(typeof(OperationType))
        .Cast<OperationType>()
        .Where(type => !types.Contains(type))
        .ToList();

    public static List<object[]> GetOperationsObjectArrayExcept(this List<OperationType> types) => types
        .GetOperationsExcept()
        .Select(type => new object[] { type })
        .ToList();
}
