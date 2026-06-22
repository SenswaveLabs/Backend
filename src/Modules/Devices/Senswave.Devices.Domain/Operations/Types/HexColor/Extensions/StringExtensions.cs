using System.Text.RegularExpressions;

namespace Senswave.Devices.Domain.Operations.Types.HexColor.Extensions;

internal static partial class StringExtensions
{
    internal static Result IsValidHexColor(this string hexColor)
    {
        if (string.IsNullOrWhiteSpace(hexColor))
            return Result.Failure(Error.Validation("InvalidEmptyString"));

        if (!hexColor.StartsWith("#"))
            return Result.Failure(Error.Validation("MissingHashPrefix"));

        if (hexColor.StartsWith("#"))
            hexColor = hexColor[1..];

        if (hexColor.Length != 3 && hexColor.Length != 6)
            return Result.Failure(Error.Validation("InvalidHexLength"));

        if (!Regex.IsMatch(hexColor, @"^[0-9a-fA-F]+$"))
            return Result.Failure(Error.Validation("InvalidPattern"));

        return Result.Success();
    }
}
