using Senswave.Abstractions.Entities;
using Senswave.Infrastructure.Validation;

namespace Senswave.Homes.Application.Homes.Features.UpdateHome;

public class UpdateHomeValidator : AbstractValidator<UpdateHomeCommand>
{
    public UpdateHomeValidator()
    {
        RuleFor(h => h.HomeId)
            .NotEmpty();

        RuleFor(h => h.UserId)
            .NotEmpty();

        RuleFor(h => h.Name)
            .StandardCharacterSetWithSpace()
            .Length(AllowedLengths.Names.MinLength, AllowedLengths.Names.MaxLength)
            .When(h => !string.IsNullOrEmpty(h.Name));

        RuleFor(h => h.Icon)
            .StandardCharacterSetWithSpace()
            .MaximumLength(AllowedLengths.Icons.MaxLength)
            .When(h => !string.IsNullOrWhiteSpace(h.Icon));

        RuleFor(h => h.Longitude)
            .InclusiveBetween(-180, 180)
            .When(x => x.Longitude != double.MinValue && !x.RemoveLocalization);

        RuleFor(h => h.Latitude)
            .InclusiveBetween(-90, 90)
            .When(x => x.Latitude != double.MinValue && !x.RemoveLocalization);
    }
}