using Senswave.Abstractions.Entities;
using Senswave.Infrastructure.Validation;

namespace Senswave.Devices.Application.Dashboards.Features.UpdateDashboard;

public class UpdateDashboardValidator : AbstractValidator<UpdateDashboardCommand>
{
    public UpdateDashboardValidator()
    {
        RuleFor(x => x.DashboardId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .Empty()
            .When(x => string.IsNullOrWhiteSpace(x.Name))
            .Length(AllowedLengths.Names.MinLength, AllowedLengths.Names.MaxLength)
            .StandardCharacterSetWithSpace()
            .When(x => !string.IsNullOrWhiteSpace(x.Name));

        RuleFor(x => x.Icon)
            .StandardCharacterSet()
            .MaximumLength(AllowedLengths.Icons.MaxLength);
    }

}