namespace Senswave.Homes.Application.Homes.Features.AssignHomeDataSource;

public class AssignHomeDataSourceValidator : AbstractValidator<AssignHomeDataSourceCommand>
{
    public AssignHomeDataSourceValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.HomeId)
            .NotEmpty();

        RuleFor(x => x.DataSourceId)
            .NotEmpty();
    }
}
