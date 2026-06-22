using FluentValidation;
using Senswave.Devices.Domain.Widgets.Types.Base;

namespace Senswave.Devices.Domain.Widgets.Types.Slider;

public class SliderWidgetValidator : BaseWidgetValidator<SliderWidget>
{
    public SliderWidgetValidator() : base()
    {
        RuleFor(x => x.Configuration.Step)
            .NotNull()
            .When(x => x.Operation.Type == Operations.Enums.OperationType.Integer);

        RuleFor(x => x.Configuration.Step)
            .NotNull()
            .When(x => x.Operation.Type == Operations.Enums.OperationType.Number);
    }
}
