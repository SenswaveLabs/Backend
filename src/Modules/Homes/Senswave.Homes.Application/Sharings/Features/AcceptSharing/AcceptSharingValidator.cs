using Senswave.Infrastructure.Validation;

namespace Senswave.Homes.Application.Sharings.Features.AcceptSharing;

public class AcceptSharingValidator : AbstractValidator<AcceptSharingCommand>
{
    public AcceptSharingValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Password)
            .MaximumLength(64)
            .StandardCharacterSet()
            .NotEmpty();
    }
}