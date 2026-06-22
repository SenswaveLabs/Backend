using Senswave.Users.Domain.Enums;

namespace Senswave.Users.Domain.Extensions;

public static class LanguageExtensions
{
    public static string ToLanguageString(this Language language)
        => language switch
        {
            Language.English => "en",
            Language.Polish => "pl",
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        };

    public static Language ToLanguageEnum(this string language)
        => language switch
        {
            "en" => Language.English,
            "pl" => Language.Polish,
            "" => Language.Empty,
            _ => Language.Invalid
        };
}
