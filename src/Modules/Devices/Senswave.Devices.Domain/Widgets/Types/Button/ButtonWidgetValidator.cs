using FluentValidation;
using Senswave.Devices.Domain.Widgets.Types.Base;

namespace Senswave.Devices.Domain.Widgets.Types.Button;

public class ButtonWidgetValidator : BaseWidgetValidator<ButtonWidget>
{
    public ButtonWidgetValidator() : base()
    {
        RuleFor(x => x.Configuration.Value)
            .Must(x => x.ToString().Length <= 512);
    }
}