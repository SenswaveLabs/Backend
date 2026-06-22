using Senswave.Users.Domain.Enums;

namespace Senswave.Users.Application.Users.UpdateSettings;

public class UpdateSettingsCommand : ICommand
{
    public Guid UserId { get; set; }
    public Language Language { get; set; } = Language.Invalid;
    public Theme Theme { get; set; } = Theme.Invalid;
}
