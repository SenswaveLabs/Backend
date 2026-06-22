namespace Senswave.Devices.Domain.Operations.Types.Number.DecimalSeparator;

public static class DecimalSeparatorTypeExtensions
{
    public static string FromDecimalSeparator(this DecimalSeparatorType delimeterType) => delimeterType switch
    {
        DecimalSeparatorType.Dot => ".",
        DecimalSeparatorType.Comma => ",",
        DecimalSeparatorType.Empty => "",
        _ => "invalid",
    };
    public static DecimalSeparatorType ToDecimalSeparator(this string delimeterString) => delimeterString switch
    {
        "." => DecimalSeparatorType.Dot,
        "," => DecimalSeparatorType.Comma,
        "" => DecimalSeparatorType.Empty,
        _ => DecimalSeparatorType.Invalid,
    };
}
