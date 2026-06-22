namespace Senswave.Infrastructure.Extensions;

public static class GuidExtensions
{
    public static Guid ToGuid(this string value)
    {
        if (Guid.TryParse(value, out var result))
        {
            return result;
        }

        return Guid.Empty;
    }
}
