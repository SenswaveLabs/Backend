using Senswave.Users.Domain.Enums;

namespace Senswave.Users.Application.Users.UpdateSettings;

public class UpdateSettingsValidator : AbstractValidator<UpdateSettingsCommand>
{
    public UpdateSettingsValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Language)
            .NotEqual(Language.Invalid);

        RuleFor(x => x.Theme)
            .NotEqual(Theme.Invalid);
    }
}
