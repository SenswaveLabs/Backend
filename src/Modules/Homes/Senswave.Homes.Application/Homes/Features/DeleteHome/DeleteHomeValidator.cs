namespace Senswave.Homes.Application.Homes.Features.DeleteHome;

public class DeleteHomeValidator : AbstractValidator<DeleteHomeCommand>
{
    public DeleteHomeValidator()
    {
        RuleFor(h => h.UserId)
            .NotEmpty();

        RuleFor(h => h.HomeId)
            .NotEmpty();
    }
}