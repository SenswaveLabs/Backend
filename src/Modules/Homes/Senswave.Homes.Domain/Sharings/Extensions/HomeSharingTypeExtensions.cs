using Senswave.Homes.Domain.Sharings.Enums;

namespace Senswave.Homes.Domain.Sharings.Extensions;

public static class HomeSharingTypeExtensions
{
    public static HomeSharingType ToHomeSharingType(this string type) => type switch
    {
        "Manage" => HomeSharingType.Manage,
        "Display" => HomeSharingType.Display,
        "" => HomeSharingType.Empty,
        _ => HomeSharingType.Invalid
    };

    public static string FromHomeSharingType(this HomeSharingType type) => type switch
    {
        HomeSharingType.Manage => "Manage",
        HomeSharingType.Display => "Display",
        HomeSharingType.Invalid => "Invalid",
        _ => string.Empty
    };
}
