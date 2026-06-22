using Senswave.Users.Application.Users.GetUser;
using Senswave.Users.Domain.Extensions;

namespace Senswave.Users.Api.Users.GetUser;

internal static class GetSettingsExtensions
{
    public static GetUserResponse ToResponse(this UserDto dto) => new()
    {
        Id = dto.User.Id,
        Email = dto.User.Email!,
        Language = dto.User.Language.ToLanguageString(),
        Theme = dto.User.Theme.ToThemeString(),
        HasActiveConsent = dto.HasActiveConsent
    };
}
