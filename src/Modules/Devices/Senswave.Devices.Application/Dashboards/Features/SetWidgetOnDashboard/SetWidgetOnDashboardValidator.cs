namespace Senswave.Devices.Application.Dashboards.Features.SetWidgetOnDashboard;

public class SetWidgetOnDashboardValidator : AbstractValidator<SetWidgetOnDashboardCommand>
{
    public SetWidgetOnDashboardValidator()
    {
        RuleFor(x => x.DashboardId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.WidgetId)
            .NotEmpty();

        RuleFor(x => x.Row)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.RowSpan)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.Column)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.ColumnSpan)
            .GreaterThanOrEqualTo(1);
    }
}
