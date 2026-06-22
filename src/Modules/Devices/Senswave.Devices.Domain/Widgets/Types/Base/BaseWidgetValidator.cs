using FluentValidation;
using Senswave.Devices.Domain.Widgets.Enums;

namespace Senswave.Devices.Domain.Widgets.Types.Base;

public class BaseWidgetValidator<T> : AbstractValidator<T> where T : BaseWidget
{
    public BaseWidgetValidator()
    {
        RuleFor(x => x.Operation.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .Length(AllowedLengths.Names.MinLength, AllowedLengths.Names.MaxLength);

        RuleFor(x => x.Type)
            .NotEqual(WidgetType.Invalid)
            .NotEqual(WidgetType.Empty);

        RuleFor(x => x)
            .Must(x => x.AllowedOperationTypes.Contains(x.Operation.Type))
            .WithMessage("Operation type is not allowed by allowed operation types.");
    }
}
