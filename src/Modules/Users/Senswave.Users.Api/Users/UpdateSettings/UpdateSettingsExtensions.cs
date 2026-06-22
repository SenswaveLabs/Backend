using Senswave.Users.Application.Users.UpdateSettings;
using Senswave.Users.Domain.Extensions;

namespace Senswave.Users.Api.Users.UpdateSettings;

internal static class UpdateSettingsExtensions
{
    public static UpdateSettingsCommand ToCommand(this UpdateSettingsRequest dto, Guid user) => new()
    {
        UserId = user,
        Language = dto.Language.ToLanguageEnum(),
        Theme = dto.Theme.ToThemeEnum(),
    };
}
