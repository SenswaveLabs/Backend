using FluentValidation;
using Senswave.Devices.Domain.Widgets.Types.Base;
using Senswave.Devices.Domain.Widgets.Types.Utils;

namespace Senswave.Devices.Domain.Widgets.Types.Display;

public sealed class DisplayWidgetValidator : BaseWidgetValidator<DisplayWidget>
{
    public DisplayWidgetValidator() : base()
    {
        RuleFor(x => x.Configuration.Unit)
            .Matches(ValidationUtils.UnitRegex);
    }
}
