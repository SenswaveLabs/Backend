using Senswave.Abstractions.Entities;
using Senswave.Infrastructure.Validation;

namespace Senswave.Devices.Application.Dashboards.Features.CreateDashboard;

public class CreateDashboardValidator : AbstractValidator<CreateDashboardCommand>
{
    public CreateDashboardValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .Length(AllowedLengths.Names.MinLength, AllowedLengths.Names.MaxLength)
            .StandardCharacterSetWithSpace()
            .When(x => !string.IsNullOrWhiteSpace(x.Name));

        RuleFor(x => x.Icon)
            .StandardCharacterSet()
            .MaximumLength(AllowedLengths.Icons.MaxLength);
    }
}