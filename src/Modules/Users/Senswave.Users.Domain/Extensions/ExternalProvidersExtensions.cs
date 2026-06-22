using Senswave.Users.Domain.Enums;

namespace Senswave.Users.Domain.Extensions;

public static class ExternalProvidersExtensions
{
    public static string FromExternalProvider(this ExternalProvider provider) => provider switch
    {
        ExternalProvider.Google => "Google",
        _ => throw new ArgumentOutOfRangeException(nameof(provider), provider, null)
    };
}
