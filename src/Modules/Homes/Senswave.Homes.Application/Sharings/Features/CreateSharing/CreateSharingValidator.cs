using Senswave.Homes.Domain.Sharings.Enums;

namespace Senswave.Homes.Application.Sharings.Features.CreateSharing;

public class CreateSharingValidator : AbstractValidator<CreateSharingCommand>
{
    public CreateSharingValidator()
    {
        RuleFor(x => x.OwnerId)
            .NotEmpty();

        RuleFor(x => x.FriendEmail)
            .EmailAddress()
            .NotEmpty();

        RuleFor(x => x.HomeId)
            .NotEmpty();

        RuleFor(x => x.SharingType)
            .NotEqual(HomeSharingType.Invalid)
            .NotEqual(HomeSharingType.Empty);
    }
}