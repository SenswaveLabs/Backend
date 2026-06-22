using Senswave.Abstractions.Entities;
using Senswave.Infrastructure.Validation;

namespace Senswave.Homes.Application.Homes.Features.CreateHome;

public class CreateHomeValidator : AbstractValidator<CreateHomeCommand>
{
    public CreateHomeValidator()
    {
        RuleFor(x => x.Name)
            .StandardCharacterSetWithSpace()
            .NotEmpty()
            .Length(AllowedLengths.Names.MinLength, AllowedLengths.Names.MaxLength);

        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Icon)
            .StandardCharacterSetWithSpace()
            .NotEmpty()
            .MaximumLength(AllowedLengths.Icons.MaxLength);

        RuleFor(h => h.Longitude)
            .InclusiveBetween(-180, 180)
            .When(x => x.Longitude != double.MinValue && x.Latitude != double.MinValue);

        RuleFor(h => h.Latitude)
            .InclusiveBetween(-90, 90)
            .When(x => x.Longitude != double.MinValue && x.Latitude != double.MinValue);
    }
}