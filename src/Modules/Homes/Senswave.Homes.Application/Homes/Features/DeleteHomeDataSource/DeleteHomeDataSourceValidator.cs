namespace Senswave.Homes.Application.Homes.Features.DeleteHomeDataSource;

public class DeleteHomeDataSourceValidator : AbstractValidator<DeleteHomeDataSourceCommand>
{
    public DeleteHomeDataSourceValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.HomeId)
            .NotEmpty();
    }
}
