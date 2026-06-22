using FluentValidation;
using Senswave.Devices.Domain.Widgets.Types.Base;
using Senswave.Devices.Domain.Widgets.Types.Utils;

namespace Senswave.Devices.Domain.Widgets.Types.TimeSeriesGraph;

public sealed class TimeSeriesGraphWidgetValidator : BaseWidgetValidator<TimeSeriesGraphWidget>
{
    public TimeSeriesGraphWidgetValidator() : base()
    {
        RuleFor(x => x.Configuration.DisplayUnit)
            .Matches(ValidationUtils.UnitRegex);

        RuleFor(x => x.Configuration.InitialNumberOfData)
            .GreaterThan(0)
            .LessThanOrEqualTo(1000);

        RuleFor(x => x.Configuration.DisplayType)
            .NotEqual(TimeSeriesDisplayType.Invalid)
            .NotEqual(TimeSeriesDisplayType.Empty);
    }
}
