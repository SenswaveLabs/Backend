namespace Senswave.Homes.Application.Sharings.Features.DeleteSharing;

public class DeleteSharingValidator : AbstractValidator<DeleteSharingCommand>
{
    public DeleteSharingValidator()
    {
        RuleFor(x => x.UserId)
           .NotEmpty();

        RuleFor(x => x.HomeSharingId)
           .NotEmpty();
    }
}