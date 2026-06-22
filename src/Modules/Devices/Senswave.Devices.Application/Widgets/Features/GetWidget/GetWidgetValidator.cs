namespace Senswave.Devices.Application.Widgets.Features.GetWidget;

public class GetWidgetValidator : AbstractValidator<GetWidgetQuery>
{
    public GetWidgetValidator()
    {
        RuleFor(x => x.WidgetId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }

}