using Senswave.Abstractions.Entities;
using Senswave.Devices.Domain.Widgets.Enums;
using Senswave.Infrastructure.Validation;

namespace Senswave.Devices.Application.Widgets.Features.CreateWidget;

public class CreateWidgetValidator : AbstractValidator<CreateWidgetCommand>
{
    public CreateWidgetValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.OperationId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .StandardCharacterSetWithSpace()
            .Length(AllowedLengths.Names.MinLength, AllowedLengths.Names.MaxLength);

        RuleFor(x => x.Type)
            .NotEqual(WidgetType.Invalid)
            .NotEqual(WidgetType.Empty);
    }
}